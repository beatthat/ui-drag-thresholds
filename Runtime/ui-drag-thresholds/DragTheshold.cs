using System;
using BeatThat.TransformPathExt;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace BeatThat.UIDragThresholds
{
    /// <summary>
    /// Define ui drag threshold in inches or cm (as opposed to pixels) for consistency across touch devices.
    ///
    /// Attach to the same component as the UI <c>EventSystem</c> you want to configure.
    /// </summary>
    [RequireComponent(typeof(EventSystem))]
	public class DragTheshold : MonoBehaviour
	{
		public bool m_debug;

		[SerializeField][FormerlySerializedAs("fBaseDPI")]
		private float m_baseDPI = 160;

		[SerializeField][FormerlySerializedAs("fBaseDT")]
		private float m_baseDragThreshold = 1;

		[SerializeField][FormerlySerializedAs("iMinDT")]
		private int m_minDragThreshold = 5;

		public enum Units
		{
			INCHES = 0,
			CM = 1,
			PIXELS_SCALED_TO_CANVAS = 2,
            PIXELS_RELATIVE_TO_BASE

		}

		[FormerlySerializedAs("m_dragThresholdInches")]
		public float m_dragThreshold = 0.2f;

		public Canvas m_canvas;

		public Units m_units = Units.PIXELS_RELATIVE_TO_BASE;

		public float dragThreshold { get { return m_dragThreshold; }  set { m_dragThreshold = value; } }
		public Units units { get { return m_units; } set { m_units = value; } }

		// Use this for initialization
		void Start()
		{
			UpdateDragThreshold();
		}

        private void UpdateDragThreshold()
		{
            var pixels = 0;

			switch(m_units) {
			case Units.PIXELS_SCALED_TO_CANVAS:
				m_canvas = m_canvas != null ? m_canvas :
					(m_canvas = GetComponentInChildren<Canvas> ()) != null ? m_canvas :
					Array.Find (FindObjectsOfType<Canvas> (), c => c.renderMode != RenderMode.WorldSpace);

				if (m_canvas == null) {
#if UNITY_EDITOR || DEBUG_UNSTRIP
					Debug.LogWarning ("[" + Time.frameCount + "][" + this.Path () + "] DragThreshold units set to PIXELS_SCALED_TO_CANVAS but no canvas set or found");
#endif
					return;
				}

				pixels = Mathf.CeilToInt(m_canvas.scaleFactor * m_dragThreshold);

				break;

			case Units.INCHES:
				pixels = Mathf.CeilToInt (this.dragThreshold * Screen.dpi);
				break;
			case Units.CM:
				pixels = Mathf.CeilToInt (this.dragThreshold * Screen.dpi * (1f / 2.54f));
				break;
            case Units.PIXELS_RELATIVE_TO_BASE:
                pixels = (int) (m_baseDragThreshold * Screen.dpi / m_baseDPI);
                if (pixels < m_minDragThreshold)
                {
                    pixels = m_minDragThreshold;
                }
                break;

			default:
#if UNITY_EDITOR || DEBUG_UNSTRIP
				Debug.LogWarning ("[" + Time.frameCount + "] " + GetType () + " unknown units value " + m_units);
#endif
				return;
			}

#if UNITY_EDITOR || DEBUG_UNSTRIP
			if(m_debug) {
				Debug.Log("[" + Time.frameCount + "] " + GetType() + " configuring drag threshold for "
					+ this.dragThreshold + " " + this.units + " to " + pixels + " pixels");
			}
#endif

			GetComponent<EventSystem>().pixelDragThreshold = pixels;
		}

	}
}

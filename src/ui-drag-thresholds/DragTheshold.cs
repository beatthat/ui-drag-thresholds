using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace BeatThat.UI.UnityUI
{
	/// <summary>
	/// Define ui drag threshold in inches or cm (as opposed to pixels) for consistency across touch devices.
	/// 
	/// Attach to the same component as the UI <c>EventSystem</c> you want to configure.
	/// </summary>
	[RequireComponent(typeof(EventSystem))]
	public class DragTheshold : MonoBehaviour 
	{
		public enum Units
		{
			INCHES = 0,
			CM = 1
		}

		[FormerlySerializedAs("m_dragThresholdInches")]
		public float m_dragThreshold = 0.2f;

		public Units m_units = Units.INCHES;

		public float dragThreshold { get { return m_dragThreshold; }  set { m_dragThreshold = value; } }
		public Units units { get { return m_units; } set { m_units = value; } }

		// Use this for initialization
		void Start () 
		{
			UpdateDragThreshold();
		}

		private void UpdateDragThreshold()
		{
			var pixels = Mathf.CeilToInt(this.dragThreshold * Screen.dpi * (this.units == Units.INCHES? 1f: 1f/2.54f)); 
			#if APE_DEBUG_UNSTRIP
			Debug.Log("[" + Time.frameCount + "] " + GetType() + " configuring drag threshold for " 
				+ this.dragThreshold + " " + this.units + " to " + pixels + " pixels");
			#endif

			GetComponent<EventSystem>().pixelDragThreshold = pixels;
		}

	}
}
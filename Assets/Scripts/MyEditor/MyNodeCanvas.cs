using UnityEditor;
using UnityEngine;

namespace MyEditor
{
    public class MyNodeCanvas : EditorWindow
    {
        [MenuItem("MyEditorWindow/NodeCanvas")]
        private static void OpenWindow()
        {
            var window = GetWindow<MyNodeCanvas>();
            window.Init();
            window.Show();
        }

        Rect  window1;
        Rect  window2;
        float panX = 0;
        float panY = 0;

        private  string[] _options = new string[] {"Option1", "Option2", "Option3", "Option4"};
        public void Init()
        {
            window1 = new Rect(100, 100, 100, 100);
            window2 = new Rect(260, 260, 100, 100);
        }

        protected void OnGUI()
        {
            GUI.BeginGroup(new Rect(panX, panY, 100000, 100000));
            DrawNodeCurve(window1, window2); // Here the curve is drawn under the windows              

            BeginWindows();
            window1 = GUI.Window(1, window1, DrawNodeWindow, "Window 1"); // Updates the Rect's when these are dragged       
            window2 = GUI.Window(2, window2, DrawNodeWindow, "Window 2");
            EndWindows();

            GUI.EndGroup();

            if (Event.current.type == EventType.MouseDown)
            {
                IsDrag = true;
                var mousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
                DragOffset = mousePos - position.position;
                OriginPan = new Vector2(panX, panY);
            }

            if (Event.current.type == EventType.MouseUp)
            {
                IsDrag = false;
            }

            if (IsDrag)
            {
                var mousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
                var localPos = mousePos - position.position;
                var offset = localPos - DragOffset;
                panX = OriginPan.x + offset.x;
                panY = OriginPan.y + offset.y;
                Repaint();
            }
        }

        private bool    IsDrag;
        private Vector2 OriginPan;
        private Vector2 DragOffset;
        
        void DrawNodeWindow(int id)
        {
            if (GUI.Button(new Rect(5, 20, 90, 20), "Hello World"))
            {
                Debug.Log("Got a click");
                Event.current.Use();
            }

            EditorGUI.Popup(new Rect(5, 40, 90, 20),0, _options);
            GUI.DrawTexture(new Rect(5, 60, 90, 20), EditorGUIUtility.whiteTexture);
            GUI.DragWindow();
        }

        void DrawNodeCurve(Rect start, Rect end)
        {
            Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
            Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);
            Vector3 startTan = startPos + Vector3.right * 50;
            Vector3 endTan = endPos + Vector3.left * 50;
            Color shadowCol = new Color(0, 0, 0, 0.06f);
            for (int i = 0; i < 3; i++) // Draw a shadow           
                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
            Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 1);
        }
    }
}
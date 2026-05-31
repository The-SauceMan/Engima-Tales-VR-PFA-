using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace utopicsense 
{
    public class ColorHelp : EditorWindow
    {
        Texture2D grandientExample;
        private Vector2 scrollPos;

        void OnEnable()
        {
            grandientExample = (Texture2D)Resources.Load("example/gradientexample", typeof(Texture2D));
        }

        void OnGUI()
        {

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos,
                                                          false,
                                                          false);

            GUIStyle style = new GUIStyle();
            style.richText = true;

            GUILayout.Label("<size=12> Here you select the <b>colors</b> to be applied to your visual effect</size>", style);
            GUILayout.Space(10.0f);
            GUILayout.Label("<size=12><b> 'Beggining color'</b> and  <b>'ending color'</b> are colors that will create a color gradient.</size>", style);
            GUILayout.Space(10.0f);
            GUILayout.Label("<size=12>For example, if the 'beginning color' is <b>blue</b>, and the 'ending color' is <b>red</b> the gradient generated would be:</size>", style);

            GUILayout.Label(grandientExample);
            GUILayout.Space(10.0f);

            GUILayout.Label("<size=12>The gradient is the <b>color over the lifetime</b> of your particle effect. In this situation, it would start blue and on time would turn purple and then red.</size>", style);
            GUILayout.Space(10.0f);
            GUILayout.Label("<size=12> Important to remember that:</size>", style);
            GUILayout.Label("<size=12> <b>Standard:</b> shows all the options available for UtopicSense, and maps the particle colors dynamically according to sound pitch.</size>", style);
            GUILayout.Label("<size=12> <b>Simple:</b> all sounds will be mapped to particles with a single color gradient without taking its pitch into consideration.</size>", style);

            GUILayout.Label("<size=12> <b>Health Warning</b>: a small percentage of people may experience a seizure when exposed to certain color patterns.</size>", style);

            GUILayout.BeginArea(new Rect(10, 300, 30, 30));
            if (GUILayout.Button("OK"))
            {
                this.Close();
            }
            GUILayout.EndArea();

            GUILayout.Label("", GUILayout.Width(400), GUILayout.Height(50));
            EditorGUILayout.EndScrollView();
        }
    }
}


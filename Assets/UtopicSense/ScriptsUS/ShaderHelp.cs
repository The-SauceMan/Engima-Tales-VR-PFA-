using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace utopicsense 
{
    public class ShaderHelp : EditorWindow
    {
        Texture2D shaderExample;
        private Vector2 scrollPos;

        void OnEnable()
        {
            shaderExample = (Texture2D)Resources.Load("example/shaderexample", typeof(Texture2D));
        }

        // Update is called once per frame
        void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos,
                                                          false,
                                                          false);
            GUIStyle style = new GUIStyle();
            style.richText = true;

            GUILayout.Label("<size=12> Here you select the <b>shader</b> to be applied to your visual effect texture</size>", style);
            GUILayout.Space(10.0f);
            GUILayout.Label("<size=12> A shader is a type of computer program that was originally used for shading (the production of <b>appropriate levels of light, darkness, and color</b> within an image)</size>", style);
            GUILayout.Space(10.0f);
            GUILayout.Label("<size=12> Color, shaders and effects all together will create a the final looking effect</size>", style);

            GUILayout.Label("<size=12> For the color <b>orange</b> the effect of the shaders from this application will be:</size>", style);

            GUILayout.Label(shaderExample);

            GUILayout.Label("<size=12> If you want the colors of the effect to be exactly the ones you selected use the <b>Default</b> shader</size>", style);

            GUILayout.Label("<size=12> Remember: when you use the shader <b>'Additive'</b>, <b>darker colors</b> tend to be <b>transparent/clear</b> using the shader.</size>", style);

            GUILayout.Label("<size=12> The <b>multiply button</b> uses the <b>'Multiply (Double)'</b> shader </size>", style);

            GUILayout.Label("<size=12> The <b>Default button</b> consists in the <b>'Sprites/Default'</b> shader </size>", style);

            GUILayout.BeginArea(new Rect(10, 520, 30, 30));
            if (GUILayout.Button("OK"))
            {
                this.Close();
            }
            GUILayout.EndArea();

            GUILayout.Label("", GUILayout.Width(400), GUILayout.Height(20));
            EditorGUILayout.EndScrollView();

        }
    }
}


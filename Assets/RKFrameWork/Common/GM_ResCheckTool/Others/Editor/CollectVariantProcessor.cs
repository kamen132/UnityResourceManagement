using GMResChecker;
using System.Collections.Generic;
using System.Text;
using UnityEditor.Build;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

class CollectVariantProcessor : IPreprocessShaders
{
    public int callbackOrder { get { return 0; } }
    public static bool CanStart = false;
    public void OnProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> data)
    {
        if(CanStart == false)
        {
            return;
        }
        Debug.Log("OnProcessShader " + shader + "Count==" + data.Count);
        for (int i = data.Count - 1; i >= 0; --i)
        {
            ShaderCompilerData item = data[i];
            ShaderKeyword[] shaderKeywords = item.shaderKeywordSet.GetShaderKeywords();
            //Debug.Log("count==" + shaderKeywords.Length);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var shaderKeyword in shaderKeywords)
            {
                //Debug.Log("shaderKeyword==" + ShaderKeyword.GetGlobalKeywordName(shaderKeyword));
                stringBuilder.Append(ShaderKeyword.GetGlobalKeywordName(shaderKeyword));
                stringBuilder.Append("    ");
            }
            stringBuilder.Append("\n");
            FileManager.AppendAllText(Application.dataPath + "/../H3dShader.txt", stringBuilder.ToString());
            //if (item.shaderKeywordSet.IsEnabled(m_Blue))
            //    continue;

            //data.RemoveAt(i);
        }
    }
}
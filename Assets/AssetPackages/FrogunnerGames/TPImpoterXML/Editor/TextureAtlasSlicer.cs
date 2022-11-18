using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Xml;

public class TextureAtlasSlicer : EditorWindow
{
    public TextureImporter importer;
    public SpriteAlignment spriteAlignment = SpriteAlignment.Center;

    [SerializeField]
    private TextAsset xmlAsset;

    private static TextureAtlasSlicer window = ScriptableObject.CreateInstance<TextureAtlasSlicer>();

    [MenuItem("CONTEXT/TextureImporter/Slice Sprite Using XML")]
    public static void SliceUsingXML(MenuCommand command)
    {
        window.importer.textureType = TextureImporterType.Sprite;
        window.importer.mipmapEnabled = false;
        window.importer.SaveAndReimport();

        window.PerformSlice();
    }

    [MenuItem("CONTEXT/TextureImporter/Slice Sprite Using XML", true)]
    public static bool ValidateSliceUsingXML(MenuCommand command)
    {
        TextureImporter textureImporter = command.context as TextureImporter;

        char[] separator = { '.' };
        string path = textureImporter.assetPath.Split(separator)[0];
        string name = path.Insert(path.Length, ".xml");
        window.xmlAsset = AssetDatabase.LoadAssetAtPath(name, typeof(TextAsset)) as TextAsset;

        window.importer = textureImporter;

        //valid only if the texture type is 'sprite' or 'advanced'.
        return window.xmlAsset ? true : false;
    }

    private void PerformSlice()
    {
        XmlDocument document = new XmlDocument();
        document.LoadXml(xmlAsset.text);

        XmlElement root = document.DocumentElement;
        if (root.Name == "TextureAtlas")
        {
            bool failed = false;

            Texture2D texture = AssetDatabase.LoadMainAssetAtPath(importer.assetPath) as Texture2D;
            int textureHeight = texture.height;

            List<SpriteMetaData> metaDataList = new List<SpriteMetaData>();

            foreach (XmlNode childNode in root.ChildNodes)
            {
                if (childNode.Name == "sprite")
                {
                    try
                    {
                        int width = Convert.ToInt32(childNode.Attributes["w"].Value);
                        int height = Convert.ToInt32(childNode.Attributes["h"].Value);
                        int x = Convert.ToInt32(childNode.Attributes["x"].Value);
                        int y = textureHeight - (height + Convert.ToInt32(childNode.Attributes["y"].Value));
                        float pX = (float)Convert.ToDouble(childNode.Attributes["pX"].Value);
                        float pY = (float)Convert.ToDouble(childNode.Attributes["pY"].Value);

                        char[] separator = { '.' };

                        SpriteMetaData spriteMetaData = new SpriteMetaData
                        {
                            alignment = (int)spriteAlignment,
                            border = new Vector4(),
                            name = childNode.Attributes["n"].Value.Split(separator)[0],
                            pivot = new Vector2(pX, pY),
                            rect = new Rect(x, y, width, height)
                        };

                        metaDataList.Add(spriteMetaData);
                    }
                    catch (Exception exception)
                    {
                        failed = true;
                        Debug.LogException(exception);
                    }
                }
                else
                {
                    Debug.LogError("Child nodes should be named 'sprite' !");
                    failed = true;
                }
            }

            if (!failed)
            {
                importer.spriteImportMode = SpriteImportMode.Multiple;
                importer.spritesheet = metaDataList.ToArray();

                EditorUtility.SetDirty(importer);

                try
                {
                    AssetDatabase.StartAssetEditing();
                    AssetDatabase.ImportAsset(importer.assetPath);
                }
                finally
                {
                    AssetDatabase.StopAssetEditing();
                }
            }
        }
        else
        {
            Debug.LogError("XML needs to have a 'TextureAtlas' root node!");
        }
    }
}

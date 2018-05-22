using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class SpriteProcessor
{
    public const int PIXELS_PER_UNIT = 32;

    [MenuItem("Assets/Sprite/Process Sprite", false, 0)]
    public static void processtSprite()
    {
        string[] guids = Selection.assetGUIDs;

        if (null == guids || guids.Length == 0)
            return;

        for (int i = 0; i < guids.Length; i++)
        {
            string l_path = AssetDatabase.GUIDToAssetPath(guids[i]);
            Texture2D l_texture = AssetDatabase.LoadAssetAtPath<Texture2D>(l_path);

            if (null == l_texture)
                continue;

            TextureImporter l_importer = AssetImporter.GetAtPath(l_path) as TextureImporter;
            l_importer.spritePixelsPerUnit = PIXELS_PER_UNIT;
            l_importer.textureType = TextureImporterType.Sprite;
            l_importer.textureCompression = TextureImporterCompression.Uncompressed;
            l_importer.filterMode = FilterMode.Point;
            l_importer.mipmapEnabled = false;

            TextureImporterSettings l_settings = new TextureImporterSettings();
            l_importer.ReadTextureSettings(l_settings);
            l_settings.spriteMeshType = SpriteMeshType.FullRect;
            l_settings.spriteExtrude = 0;
            l_importer.SetTextureSettings(l_settings);

            l_importer.SaveAndReimport();
        }


        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/Sprite/Slice Sprite", false, 50)]
    public static void sliceSprite()
    {
        processtSprite();

        string[] guids = Selection.assetGUIDs;

        if (null == guids || guids.Length == 0)
            return;

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

            if (null == texture)
                continue;

            if (!texture.name.Contains("_"))
            {
                Debug.LogWarningFormat("Could not process slice, asset needs an appended _WxH at the end of it's name: {0}", texture.name);
                return;
            }

            string[] split = texture.name.Split('_');

            if (split.Length != 2)
            {
                Debug.LogWarningFormat("Could not process slice, name is in incorrect format: {0}", texture.name);
            }

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            importer.isReadable = true;
            importer.spriteImportMode = SpriteImportMode.Multiple;

            string size = split[1];

            int width = int.Parse(size.Split('x')[0]);
            int height = int.Parse(size.Split('x')[1]);

            int rows = texture.height / height;
            int columns = texture.width / width;

            List<SpriteMetaData> metas = new List<SpriteMetaData>();

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    SpriteMetaData meta = new SpriteMetaData();
                    meta.rect = new Rect(c * width, r * height, width, height);
                    meta.name = string.Format("{0}_{1}x{2}", texture.name, c, r);
                    metas.Add(meta);
                }
            }

            importer.spritesheet = metas.ToArray();
            importer.SaveAndReimport();
        }



        AssetDatabase.Refresh();
    }
}

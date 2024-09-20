using UnityEngine;
using TMPro;

public class TextColorAndSizeChanger : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;

    void Start()
    {
        // Get reference to TextMeshProUGUI component
        textMeshPro = GetComponent<TextMeshProUGUI>();

        // Set the text to demonstrate the POC
        textMeshPro.text = "This is a test for changing color and size!";

        // Call the method to change vertex colors and sizes
        ModifyTextMesh();
    }
    private void Update()
    {
        ModifyTextMesh();
    }
    void ModifyTextMesh()
    {
        // Force update to populate textInfo
        textMeshPro.ForceMeshUpdate();

        // Get the TextMeshPro mesh info
        TMP_TextInfo textInfo = textMeshPro.textInfo;

        // Loop through each character in the text
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            // Get character info
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            // Only process visible characters
            if (!charInfo.isVisible) continue;

            // Determine the color and size based on character index
            Color32 color = (i % 2 == 0) ? Color.red : Color.green;
            float size = (i % 2 == 0) ? 1f : 1.5f;

            // Get the material index and vertex index for this character
            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            // Set the color of the character's vertices
            Color32[] vertexColors = textInfo.meshInfo[materialIndex].colors32;
            for (int j = 0; j < 4; j++)
            {
                vertexColors[vertexIndex + j] = color;
            }

            // Set the size of the character by scaling the vertices
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;
            Vector3 offset = (vertices[vertexIndex + 0] + vertices[vertexIndex + 2]) / 2;
            for (int j = 0; j < 4; j++)
            {
                vertices[vertexIndex + j] = (vertices[vertexIndex + j] - offset) * size + offset;
            }
        }

        // Update the mesh with the new colors and vertex positions
        textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32 | TMP_VertexDataUpdateFlags.Vertices);
    }
}

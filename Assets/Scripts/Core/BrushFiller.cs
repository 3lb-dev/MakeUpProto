using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BrushFiller : MonoBehaviour
{
    public GameObject brushPrefab;

    //public Vector2 brushSize = Vector2.one * 0.5f;
    //public Vector2 spacing = Vector2.one * 0.1f;

    //[ContextMenu(nameof(PopulateSprite))]
    public List<MaskBrush> PopulateSprite(List<SpriteRenderer> spriteRenderers, MakeUpPart v_part)
    {
        int index = 0;
        List<MaskBrush> maskBrushes = new List<MaskBrush>();
        foreach (SpriteRenderer SR in spriteRenderers)
        {
            var sprite = SR.sprite;
            var texture = sprite.texture;
            // get the world space dimensions
            var worldBounds = SR.bounds; // <--- De donde a donde se instancian los colliders
            // get the pixel space dimensions
            var pixelRect = sprite.rect;

            // Multiply by this factor to convert world space size to pixels
            var fillerSizeFactor = Vector2.one / worldBounds.size * pixelRect.size;
            var fillerSizeInPixels = Vector2Int.RoundToInt(v_part.brushSize * fillerSizeFactor);

            //var spacingSizeFactor = Vector2.one / worldBounds.size * pixelRect.size;
            //var spacingSizeInPixels = Vector2Int.RoundToInt(spacing * spacingSizeFactor);

            var start = worldBounds.min;
            var end = worldBounds.max;

            Vector2 pixelStart = v_part.pixelCoordinatesInAtlas; // DEBUG
            Vector2 spriteDimensions = v_part.dimensionsWithinAtlas; // DEBUG
            Vector2 pixelEnd = pixelStart + spriteDimensions;
            Vector2 pixelCoordinates = pixelStart;

            // Use proper for loops instead of ugly and error prone while ;)
            for (var worldY = start.y; worldY < end.y; worldY += v_part.brushSize.y /*+ spacing.y*/)
            {
                // convert the worldY to pixel coordinate
                var pixelY = Mathf.RoundToInt((worldY - worldBounds.center.y + worldBounds.size.y / 2f) * fillerSizeFactor.y);

                // quick safety check if this fits into the texture pixel space
                if (pixelY + fillerSizeInPixels.y >= texture.height)
                {
                    continue;
                }
                if (pixelCoordinates.y >= pixelEnd.y - fillerSizeInPixels.y && v_part.shouldRemoveLastRow)
                {
                    continue;
                }

                for (var worldX = start.x; worldX < end.x; worldX += v_part.brushSize.x /*+ spacing.x*/)
                {
                    // convert worldX to pixel coordinate
                    var pixelX = Mathf.RoundToInt((worldX - worldBounds.center.x + worldBounds.size.x / 2f) * fillerSizeFactor.x);

                    // again the check if this fits into the texture pixel space
                    if (pixelX + fillerSizeInPixels.x >= texture.width)
                    {
                        continue;
                    }
                    if (pixelCoordinates.x >= pixelEnd.x - fillerSizeInPixels.x && v_part.shouldRemoveLastColumn)
                    {
                        continue;
                    }

                    // Cut out a rectangle from the texture at given pixel coordinates
                    var pixels = texture.GetPixels(Mathf.RoundToInt(pixelCoordinates.x), Mathf.RoundToInt(pixelCoordinates.y), fillerSizeInPixels.x, fillerSizeInPixels.y);
                    //Debug.LogWarning(index + "- Values: " + pixelStart);
                    //var pixels = texture.GetPixels(pixelX, pixelY, fillerSizeInPixels.x, fillerSizeInPixels.y);
                    pixelCoordinates = new Vector2(pixelCoordinates.x + fillerSizeInPixels.x, pixelCoordinates.y);
                    //var pixels = texture.GetPixels(pixelX, pixelY, fillerSizeInPixels.x, fillerSizeInPixels.y);
                    //7310, 7

                    // Using Linq to check if all pixels are transparent
                    if (pixels.All(p => Mathf.Approximately(p.a, 0f)))
                    {
                        continue;
                    }

                    // otherwise spawn a filler here
                    GameObject newBrushObject = Instantiate(brushPrefab, new Vector3(worldX, worldY, 0), Quaternion.identity, transform) as GameObject;
                    newBrushObject.transform.localScale = new Vector3(v_part.brushSize.x, v_part.brushSize.y, 1);
                    MaskBrush newBrush = newBrushObject.GetComponent<MaskBrush>();
                    newBrush.SetupBrush(v_part);
                    maskBrushes.Add(newBrush);
                }
                index++;
                pixelCoordinates = new Vector2(pixelStart.x, pixelCoordinates.y + fillerSizeInPixels.y);
            }
        }
        return maskBrushes;
    }
}

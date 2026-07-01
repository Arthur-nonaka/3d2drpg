using UnityEngine;
using UnityEngine.UI;

public class UIGradient : BaseMeshEffect
{
    public enum GradientDirection
    {
        TopToBottom,
        BottomToTop,
        LeftToRight,
        RightToLeft,
    }

    public Gradient gradient;
    public GradientDirection direction;

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
            return;

        var rect = graphic.rectTransform.rect;
        var vertex = new UIVertex();

        for (int i = 0; i < vh.currentVertCount; i++)
        {
            vh.PopulateUIVertex(ref vertex, i);

            float t = direction switch
            {
                GradientDirection.TopToBottom => Mathf.InverseLerp(
                    rect.yMax,
                    rect.yMin,
                    vertex.position.y
                ),
                GradientDirection.BottomToTop => Mathf.InverseLerp(
                    rect.yMin,
                    rect.yMax,
                    vertex.position.y
                ),
                GradientDirection.LeftToRight => Mathf.InverseLerp(
                    rect.xMin,
                    rect.xMax,
                    vertex.position.x
                ),
                GradientDirection.RightToLeft => Mathf.InverseLerp(
                    rect.xMax,
                    rect.xMin,
                    vertex.position.x
                ),
                _ => 0,
            };

            vertex.color = gradient.Evaluate(t);
            vh.SetUIVertex(vertex, i);
        }
    }
}

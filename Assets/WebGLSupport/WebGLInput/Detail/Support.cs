using UnityEngine;

namespace WebGLSupport.Detail
{
    public static class Support
    {
        /// <summary>
        /// 画面内の描画範囲を取得する
        /// </summary>
        /// <param name="uiElement"></param>
        /// <returns></returns>
        public static Rect GetScreenCoordinates(RectTransform uiElement)
        {
            var worldCorners = new Vector3[4];
            uiElement.GetWorldCorners(worldCorners);

            // try to support RenderMode:WorldSpace
            var canvas = uiElement.GetComponentInParent<Canvas>();
            var useCamera = (canvas.renderMode != RenderMode.ScreenSpaceOverlay);
            if (canvas && useCamera)
            {
                var camera = canvas.worldCamera;
                if (!camera) camera = Camera.main;

                for (var i = 0; i < worldCorners.Length; i++)
                {
                    worldCorners[i] = camera.WorldToScreenPoint(worldCorners[i]);
                }
            }

            var min = new Vector3(float.MaxValue, float.MaxValue);
            var max = new Vector3(float.MinValue, float.MinValue);
            for (var i = 0; i < worldCorners.Length; i++)
            {
                min.x = Mathf.Min(min.x, worldCorners[i].x);
                min.y = Mathf.Min(min.y, worldCorners[i].y);
                max.x = Mathf.Max(max.x, worldCorners[i].x);
                max.y = Mathf.Max(max.y, worldCorners[i].y);
            }

            return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
        }

    }
}
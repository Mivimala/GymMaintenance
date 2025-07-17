using Emgu.CV.Structure;
using Emgu.CV.Util;
using GymMaintenance.Model.Entity;
using Newtonsoft.Json;
using System.Drawing;

namespace GymMaintenance.Helper
{
    public static class KeypointUtils
    {
        public static string SerializeKeypoints(VectorOfKeyPoint keypoints)
        {
            var list = keypoints.ToArray().Select(kp => new KeyPointDto
            {
                X = kp.Point.X,
                Y = kp.Point.Y,
                Size = kp.Size,
                Angle = kp.Angle,
                Response = kp.Response,
                Octave = kp.Octave,
                ClassId = kp.ClassId
            }).ToList();

            return JsonConvert.SerializeObject(list);
        }

        public static VectorOfKeyPoint DeserializeKeypoints(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return new VectorOfKeyPoint();

            var list = JsonConvert.DeserializeObject<List<KeyPointDto>>(json);
            var vector = new VectorOfKeyPoint();

            foreach (var kp in list)
            {
                var mkey = new MKeyPoint
                {
                    Point = new PointF(kp.X, kp.Y),
                    Size = kp.Size,
                    Angle = kp.Angle,
                    Response = kp.Response,
                    Octave = kp.Octave,
                    ClassId = kp.ClassId
                };
                vector.Push(new[] { mkey });
            }

            return vector;
        }
    }
}

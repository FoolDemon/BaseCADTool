using System;
using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;
using System.Text;

namespace BaseNetCADTool
{
    public static class BaseTool
    {
        /// <summary>
        /// 角度转换为弧度
        /// </summary>
        /// <param name="Degree">角度值</param>
        /// <returns>弧度</returns>
        public static double DegreeToAngle(this Double degree)
        {
            return degree * Math.PI / 180;
        }
        /// <summary>
        /// 弧度转换为角度
        /// </summary>
        /// <param name="angle">弧度值</param>
        /// <returns>角度</returns>
        public static double AngleToDegree(this Double angle)
        {
            return angle * 180 / Math.PI;
        }
        /// <summary>
        /// 判断3点是否在同一条直线上
        /// </summary>
        /// <param name="startPoint3d">起点</param>
        /// <param name="secondPoint3d">第二点</param>
        /// <param name="endPoint3d">终点</param>
        /// <returns>bool</returns>
        public static bool IsOnOneLine(this Point3d startPoint3d, Point3d secondPoint3d, Point3d endPoint3d)
        {
            Vector3d V1_2 = secondPoint3d.GetVectorTo(startPoint3d);
            Vector3d V2_3 = secondPoint3d.GetVectorTo(endPoint3d);
            if (V1_2.GetAngleTo(V2_3) == 0 || V1_2.GetAngleTo(V2_3) == Math.PI)
            { return true; }
            else { return false; }
        }
        /// <summary>
        /// 根据两点的坐标获取角度值
        /// </summary>
        /// <param name="startPoint">起始坐标</param>
        /// <param name="endPoint">终点坐标</param>
        /// <returns>角度值</returns>
        public static double GetAngle(this Point3d startPoint, Point3d endPoint)
        {
            //声明一个与X轴平行的向量
            Vector3d X_Vector3d = new Vector3d(1, 0, 0);
            //获取起点到终点的向量
            Vector3d vector = startPoint.GetVectorTo(endPoint);
            return vector.Y > 0 ? X_Vector3d.GetAngleTo(vector) : -X_Vector3d.GetAngleTo(vector);
        }
        /// <summary>
        /// 获取两点之间的距离
        /// </summary>
        /// <param name="startPoint">起点</param>
        /// <param name="endPoint">终点</param>
        /// <returns></returns>
        public static double GetDistanceBetweenToPoint(this Point3d startPoint, Point3d endPoint)
        {
            double Distance = (startPoint.X - endPoint.X) * (startPoint.X - endPoint.X) + (startPoint.Y - endPoint.Y) * (startPoint.Y - endPoint.Y) + (startPoint.Z - endPoint.Z) * (startPoint.Z - endPoint.Z);
            return Math.Sqrt(Distance);
        }
        /// <summary>
        /// 获取两点之间的中心点
        /// </summary>
        /// <param name="startPoint">起点</param>
        /// <param name="endPoint">终点</param>
        /// <returns>中心点</returns>
        public static Point3d GetCenterPointBetweenTwoPoint(this Point3d startPoint, Point3d endPoint)
        {
            return new Point3d((startPoint.X + endPoint.X) / 2, (startPoint.Y + endPoint.Y) / 2, (startPoint.Z + endPoint.Z) / 2);
        }

    }
}

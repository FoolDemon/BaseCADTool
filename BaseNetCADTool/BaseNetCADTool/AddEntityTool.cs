using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;
using System.Text;

namespace BaseNetCADTool
{
    public static class AddEntityTool
    {
        /// <summary>
        /// 将图形对象添加到图形数据库中
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="entity">图形对象</param>
        /// <returns>图形的ObjectId</returns>
        public static ObjectId AddEntityToModelSpace(this Database db, Entity entity)
        {
            //创建对象id变量用于接受 返回id
            ObjectId entId = ObjectId.Null;
            //开启事物处理   Transaction:事物
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                // 打开块表        Block 块   db.BlockTableId:数据库块表Id
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                //打开块表记录         Record :档案、记录  
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                //将图形添加到块表记录  并接收对象Id  Append:追加
                entId = btr.AppendEntity(entity);
                //更新数据信息   Newly:最近、重新
                trans.AddNewlyCreatedDBObject(entity, true);
                //提交事物
                trans.Commit();
            }
            return entId;
        }
        /// <summary>
        /// 将图形对象添加到图形文件中
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="entity">图形对象，可变参数</param>
        /// <returns>图形的ObjectId</returns>
        public static ObjectId[] AddEntityToModelSpace(this Database db, params Entity[] entity)
        {
            //创建对象id变量用于接受 返回id
            ObjectId[] entId = new ObjectId[entity.Length];
            //开启事物处理   Transaction:事物
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                // 打开块表        Block 块   db.BlockTableId:数据库块表Id
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                //打开块表记录         Record :档案、记录  
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                //将图形添加到块表记录  并接收对象Id  Append:追加
                for (int i = 0; i < entity.Length; i++)
                {
                    entId[i] = btr.AppendEntity(entity[i]);
                    //更新数据信息   Newly:最近、重新
                    trans.AddNewlyCreatedDBObject(entity[i], true);
                }
                //提交事物
                trans.Commit();
            }
            return entId;
        }
        /// <summary>
        /// 绘制直线
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="startPoint">起点坐标</param>
        /// <param name="endPoint">终点坐标</param>
        /// <returns></returns>
        public static ObjectId AddEntityToModelSpace(this Database db, Point3d startPoint, Point3d endPoint)
        {
            // Line line1 = new Line(startPoint, endPoint);
            return AddEntityTool.AddEntityToModelSpace(db, new Line(startPoint, endPoint));
        }
        /// <summary>
        /// 绘制直线
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="startPoint">起点坐标</param>
        /// <param name="length">直线长度</param>
        /// <param name="degree">直线弧度</param>
        /// <returns>ObjectId</returns>
        public static ObjectId AddEntityToModelSpace(this Database db, Point3d startPoint, Double length, Double degree)
        {
            //计算终点坐标
            Double End_X = startPoint.X + length * Math.Cos(degree.DegreeToAngle());
            Double End_Y = startPoint.Y + length * Math.Sin(degree.DegreeToAngle());
            //Point3d
            Point3d endLine = new Point3d(End_X, End_Y, 0);
            return AddEntityTool.AddEntityToModelSpace(db, new Line(startPoint, endLine));
        }
        /// <summary>
        /// 绘制圆弧
        /// </summary>
        /// <param name="db">图形数据库对象</param>
        /// <param name="center">圆心点位置</param>
        /// <param name="radius">半径</param>
        /// <param name="startDegree">起始角度</param>
        /// <param name="endDegree">终止角度</param>
        /// <returns>ObjectId</returns>
        public static ObjectId AddArcToModelSpace(this Database db, Point3d center, double radius, double startDegree, double endDegree)
        {

            return AddEntityTool.AddEntityToModelSpace(db, new Arc(center, radius, startDegree.DegreeToAngle(), endDegree.DegreeToAngle()));
        }
        /// <summary>
        /// 三点绘制圆弧
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="startPoint3d">起点坐标</param>
        /// <param name="pointOnArc">圆弧上的点</param>
        /// <param name="endPoint3d">重点坐标</param>
        /// <returns></returns>
        public static ObjectId AddArcToModelSpace(this Database db, Point3d startPoint3d, Point3d pointOnArc, Point3d endPoint3d)
        {
            //判断三点是否在同一条直线上
            if (startPoint3d.IsOnOneLine(pointOnArc, endPoint3d))
            {
                Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
                ed.WriteMessage("3个顶点在同一条直线上，无法绘制圆弧");
                return ObjectId.Null;
            }
            //创建几何对象
            CircularArc3d CArc = new CircularArc3d(startPoint3d, pointOnArc, endPoint3d);
            //获取几何对象的属性
            double radius = CArc.Radius; //半径
            Point3d conter = CArc.Center;//圆心

            //Vector3d cs = conter.GetVectorTo(startPoint3d);//圆心与第一个点构成向量
            //Vector3d ce = conter.GetVectorTo(endPoint3d);//圆心与第二个点构成向量
            //Vector3d xVector = new Vector3d(1, 0, 0);//X正方向的向量

            ////获取角度
            //double startAngle = cs.Y>0 ? xVector.GetAngleTo(cs): -xVector.GetAngleTo(cs);//圆心与第一个点的角度
            //double endAngle = ce.Y>0 ? xVector.GetAngleTo(ce): -xVector.GetAngleTo(ce);//圆心与第二个点的角度
            //获取角度
            double startAngle = conter.GetAngle(startPoint3d);
            double endAngle = conter.GetAngle(endPoint3d);
            //创建圆弧对象
            Arc arc = new Arc(conter, radius, startAngle, endAngle);
            return AddEntityToModelSpace(db, arc);//添加到图形数据库中

        }
        /// <summary>
        /// 根据圆心、起点、角度绘制圆弧
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="center">圆心</param>
        /// <param name="startPoint3d">起点</param>
        /// <param name="degree">夹角值(角度值)</param>
        /// <returns></returns>
        public static ObjectId AddArcToModelSpace(this Database db, Point3d center, Point3d startPoint3d, double degree)
        {
            //获取半径
            double radius = center.GetDistanceBetweenToPoint(startPoint3d);
            //获取起始角度
            double startAngle = center.GetAngle(startPoint3d);
            //声明圆弧对象  //角度转换为弧度
            Arc arc = new Arc(center, radius, startAngle, startAngle + degree.DegreeToAngle());
            //添加到图形数据库中
            return AddEntityToModelSpace(db, arc);
        }
        /// <summary>
        /// 根据圆心、半径绘制圆(默认XY平面)
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="centrePoint3d">圆心坐标</param>
        /// <param name="radius">半径</param>
        /// <returns></returns>
        public static ObjectId AddCircleToModelSpace(this Database db, Point3d centrePoint3d, double radius)
        {
            return AddEntityToModelSpace(db, new Circle(centrePoint3d, new Vector3d(0, 0, 1), radius));
        }
        /// <summary>
        /// 两点绘制圆(两点的中心为圆心)
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="point1">起点</param>
        /// <param name="point2">终点</param>
        /// <returns>对象Id</returns>
        public static ObjectId AddCircleToModelSpace(this Database db, Point3d point1, Point3d point2)
        {
            //获取中心点
            Point3d centerPoint = point1.GetCenterPointBetweenTwoPoint(point2);
            //获取半径
            double radius = centerPoint.GetDistanceBetweenToPoint(point1);
            return AddCircleToModelSpace(db, centerPoint, radius);
        }
        /// <summary>
        /// 三点绘制圆
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="point1">第一个点</param>
        /// <param name="point2">第二个点</param>
        /// <param name="point3">第三个点</param>
        /// <returns>对象Id</returns>
        public static ObjectId AddCircleToModelSpace(this Database db, Point3d point1, Point3d point2, Point3d point3)
        {
            //判断三点是否在同一条直线上
            if (point1.IsOnOneLine(point2, point3))
            {
                Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
                ed.WriteMessage("3个顶点在一条直线上，无法绘制圆");
                return ObjectId.Null;
            }
            //创建几何类CircularArc3d对象
            CircularArc3d CArc = new CircularArc3d(point1, point2, point3);
            return AddCircleToModelSpace(db, CArc.Center, CArc.Radius);
        }
        /// <summary>
        /// 绘制多段线直线(无弧度)
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="isClosed">是否闭合</param>
        /// <param name="constantWidth">线宽</param>
        /// <param name="vertices">多段线顶点</param>
        /// <returns>对象Id</returns>
        public static ObjectId AddPolyLineToModelSpace(this Database db, bool isClosed, double constantWidth, params Point2d[] vertices)
        {
            //判断顶点是否大于两个
            if (vertices.Length < 2)
            {
                Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
                ed.WriteMessage("顶点少于2，无法绘制多段线");
                return ObjectId.Null;
            }
            //声明多段线对象
            Polyline pl = new Polyline();
            for (int i = 0; i < vertices.Length; i++)
            {
                //添加多段线的顶点
                pl.AddVertexAt(i, vertices[i], 0, 0, 0);
            }
            //判断是否闭合
            if (isClosed)
            {
                pl.Closed = true;
            }
            //设置多段线线宽
            pl.ConstantWidth = constantWidth;
            return AddEntityToModelSpace(db, pl);
        }
        /// <summary>
        /// 两点绘制矩形
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="startPoint2d">起点</param>
        /// <param name="endPoint2d">终点</param>
        /// <param name="constantWidth">线宽</param>
        /// <returns></returns>
        public static ObjectId AddRectToModelSpace(this Database db, Point2d startPoint2d, Point2d endPoint2d, double constantWidth = 0)
        {
            //申明多段线
            Polyline pl = new Polyline();
            //计算矩形的四个坐标顶点
            Point2d p1 = new Point2d(Math.Min(startPoint2d.X, endPoint2d.X), Math.Min(startPoint2d.Y, endPoint2d.Y));
            Point2d p2 = new Point2d(Math.Max(startPoint2d.X, endPoint2d.X), Math.Min(startPoint2d.Y, endPoint2d.Y));
            Point2d p3 = new Point2d(Math.Max(startPoint2d.X, endPoint2d.X), Math.Max(startPoint2d.Y, endPoint2d.Y));
            Point2d p4 = new Point2d(Math.Min(startPoint2d.X, endPoint2d.X), Math.Max(startPoint2d.Y, endPoint2d.Y));
            //添加顶点
            pl.AddVertexAt(0, p1, 0, 0, 0);
            pl.AddVertexAt(1, p2, 0, 0, 0);
            pl.AddVertexAt(2, p3, 0, 0, 0);
            pl.AddVertexAt(3, p4, 0, 0, 0);
            pl.Closed = true;
            pl.ConstantWidth = constantWidth;
            return AddEntityToModelSpace(db, pl);
        }
        /// <summary>
        /// 绘制多边形(内切)
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="center">圆心点</param>
        /// <param name="radius">半径</param>
        /// <param name="sideNum">边数</param>
        /// <param name="startDergee">起始角度</param>
        /// <returns>ObjectId</returns>
        public static ObjectId AddPolygonToModelSpace(this Database db, Point2d center, double radius, short sideNum, double startDergee = 90)
        {
            //声明多段线对象
            Polyline polyline = new Polyline();
            //判断边数是否小于3
            if (sideNum < 3)
            {
                Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
                ed.WriteMessage("边数少于3，无法绘制多变形");
                return ObjectId.Null;
            }
            //声明角度变量  用于保存每次半径的角度
            double angle = startDergee.DegreeToAngle();
            //声明二维顶点数组
            Point2d[] PointS = new Point2d[sideNum];
            for (int i = 0; i < sideNum; i++)
            {
                //设置多边形的顶点
                PointS[i] = new Point2d(center.X + Math.Cos(angle) * radius, center.Y + Math.Sin(angle) * radius);
                //添加多段线顶点
                polyline.AddVertexAt(i, PointS[i], 0, 0, 0);
                //设置第i条边的角度
                angle += (Math.PI * 2) / sideNum;
            }
            //设置线段闭合
            polyline.Closed = true;
            return AddEntityToModelSpace(db, polyline);
        }
        /// <summary>
        /// 绘制椭圆
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="center">圆心点</param>
        /// <param name="majorRadius">长轴长度值</param>
        /// <param name="shortRadius">短轴长度值</param>
        /// <param name="degree">旋转角度</param>
        /// <returns>ObjectId</returns>
        public static ObjectId AddEllipseToModelSpace(this Database db, Point3d center, double majorRadius, double shortRadius, double degree)
        {
            //Ellipse(Point3d center, Vector3d unitNormal, Vector3d majorAxis, double radiusRatio, double startAngle, double endAngle);
            //Ellipse(圆心坐标，垂直平面的向量，圆心到椭圆长轴长的向量，短轴与长轴的比例，起始角度，终止角度)
            //计算短轴与长轴的比 两项之比不可以大于1
            double Ratio = shortRadius / majorRadius;
            //声明3D 平面向量
            Vector3d MajorAxis = new Vector3d(majorRadius * Math.Cos(degree.DegreeToAngle()), majorRadius * Math.Sin(degree.DegreeToAngle()), 0);
            //声明椭圆对象
            Ellipse el = new Ellipse(center, Vector3d.ZAxis, MajorAxis, Ratio, 0, 2 * Math.PI);
            return AddEntityToModelSpace(db, el);
        }
        /// <summary>
        /// 绘制(椭圆的)圆弧
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="center">圆心点</param>
        /// <param name="majorRadius">长轴长度值</param>
        /// <param name="shortRadius">短轴长度值</param>
        /// <param name="degree">旋转角度</param>
        /// <param name="startDegree">起始角度</param>
        /// <param name="endDegree">终止角度</param>
        /// <returns>ObjectId</returns>
        public static ObjectId AddEllipseToModelSpace(this Database db, Point3d center, double majorRadius, double shortRadius, double degree, double startDegree, double endDegree)
        {
            //计算短轴与长轴的比 两项之比不可以大于1
            double Ratio = shortRadius / majorRadius;
            //声明3D 平面向量
            Vector3d MajorAxis = new Vector3d(majorRadius * Math.Cos(degree.DegreeToAngle()), majorRadius * Math.Sin(degree.DegreeToAngle()), 0);
            //声明椭圆对象
            Ellipse el = new Ellipse(center, Vector3d.ZAxis, MajorAxis, Ratio, startDegree.DegreeToAngle(), endDegree.DegreeToAngle());
            return AddEntityToModelSpace(db, el);
        }
        /// <summary>
        /// 绘制椭圆
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="majorRadius1">长轴端点1</param>
        /// <param name="majorRadius2">长轴端点2</param>
        /// <param name="shortRadius">短轴的长度</param>
        /// <returns>ObjectId</returns>
        public static ObjectId AddEllipseToModelSpace(this Database db, Point3d majorRadius1, Point3d majorRadius2, double shortRadius)
        {
            Point3d center = majorRadius1.GetCenterPointBetweenTwoPoint(majorRadius2);
            //获取短轴与长轴的比例 不能大于1 否则会生成原点的一个圆形
            double ratio = (2 * shortRadius) / majorRadius1.GetDistanceBetweenToPoint(majorRadius2);
            Vector3d MajorAxis = majorRadius1.GetVectorTo(center);
            Ellipse ellipse = new Ellipse(center, Vector3d.ZAxis, MajorAxis, ratio, 0, 2 * Math.PI);
            return AddEntityToModelSpace(db, ellipse);

        }
        /// <summary>
        /// 绘制椭圆(矩形内切圆)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="point1">矩形对角顶点1</param>
        /// <param name="point2">矩形对角顶点1</param>
        /// <returns>ObjectId</returns>
        public static ObjectId AddEllipseToModelSpace(this Database db, Point3d point1, Point3d point2)
        {
            //获取圆心点
            Point3d center = point1.GetCenterPointBetweenTwoPoint(point2);
            //获取长轴与短轴的长度
            double major_Y = Math.Abs(point1.Y - point2.Y);
            double major_X = Math.Abs(point1.X - point2.X);
            //设置短轴与长轴的比例  不能大于1 
            double ratio = major_Y / major_X;
            Vector3d MajorAxis = new Vector3d(major_X, 0, 0);
            if ((major_Y / major_X) > 1)
            {
                ratio = major_X / major_Y;
                MajorAxis = new Vector3d(0, major_Y, 0);
            }
            Ellipse ellipse = new Ellipse(center, Vector3d.ZAxis, MajorAxis, ratio, 0, 2 * Math.PI);
            return AddEntityToModelSpace(db, ellipse);
        }
    }
}

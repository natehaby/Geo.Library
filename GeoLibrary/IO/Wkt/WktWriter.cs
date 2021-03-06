﻿using System;
using System.Globalization;
using System.Text;
using GeoLibrary.Model;

namespace GeoLibrary.IO.Wkt
{
    public static class WktWriter
    {
        public static string Write(Geometry geometry)
        {
            if (geometry?.IsValid != true)
            {
                throw new ArgumentException("Invalid geometry");
            }

            var builder = new StringBuilder();
            Build(builder, geometry);

            return builder.ToString();
        }

        public static string ToWkt(this Geometry geometry)
        {
            return Write(geometry);
        }

        private static void Build(StringBuilder builder, Geometry geometry)
        {
            switch (geometry)
            {
                case Point point:
                    BuildPoint(builder, point);
                    return;
                case MultiPoint multiPoint:
                    BuildMultiPoint(builder, multiPoint);
                    return;
                case LineString lineString:
                    BuildLineString(builder, lineString);
                    return;
                case Polygon polygon:
                    BuildPolygon(builder, polygon);
                    return;
                case MultiPolygon multiPolygon:
                    BuildMultiPolygon(builder, multiPolygon);
                    return;
                default:
                    throw new ArgumentException($"Not supported geometry type: {geometry.GetType()}");
            }
        }

        private static void BuildPoint(StringBuilder builder, Point point)
        {
            builder.Append(WktTypes.Point);
            builder.Append(" (");
            BuildPointInner(builder, point);
            builder.Append(")");
        }

        private static void BuildPointInner(StringBuilder builder, Point point)
        {
            builder.Append(point.Longitude.ToString(CultureInfo.InvariantCulture));
            builder.Append(" ");
            builder.Append(point.Latitude.ToString(CultureInfo.InvariantCulture));
        }

        private static void BuildMultiPoint(StringBuilder builder, MultiPoint multiPoint)
        {
            builder.Append(WktTypes.MultiPoint);
            builder.Append(" (");
            for (var i = 0; i < multiPoint.Count; i++)
            {
                if (i > 0) builder.Append(", ");

                BuildPointInner(builder, multiPoint[i] as Point);
            }
            builder.Append(")");
        }

        private static void BuildLineString(StringBuilder builder, LineString lineString, bool skipHeader = false)
        {
            if (skipHeader == false)
            {
                builder.Append(WktTypes.LineString).Append(" ");
            }

            builder.Append("(");
            for (var i = 0; i < lineString.Count; i++)
            {
                if (i > 0) builder.Append(", ");

                BuildPointInner(builder, lineString[i]);
            }

            builder.Append(")");
        }

        private static void BuildPolygon(StringBuilder builder, Polygon polygon, bool skipHeader = false)
        {
            if (skipHeader == false)
            {
                builder.Append(WktTypes.Polygon).Append(" ");
            }

            builder.Append("(");
            for (var i = 0; i < polygon.Count; i++)
            {
                if (i > 0) builder.Append(", ");

                BuildLineString(builder, polygon[i], true);
            }

            builder.Append(")");
        }

        private static void BuildMultiPolygon(StringBuilder builder, MultiPolygon multiPolygon)
        {
            builder.Append(WktTypes.MultiPolygon).Append(" (");
            for (var i = 0; i < multiPolygon.Count; i++)
            {
                if (i > 0) builder.Append(", ");

                BuildPolygon(builder, multiPolygon[i] as Polygon, true);
            }

            builder.Append(")");
        }
    }
}

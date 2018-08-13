using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlTypes;

/* ***************************************THIS ONLY WORKED WITH SQL 2005 WHEN Compiled as a stand alone assembly on .NET 3****************************
 CREATE ASSEMBLY Geographic from 'C:\UcbManagementInformationSQL\UcbManagementInformationSQL\bin\Debug\UcbManagementInformationSQL.dll' WITH PERMISSION_SET = SAFE

CREATE PROCEDURE ConvertNationalGridToLongLat
(
@xCoord NVARCHAR(10),
@yCoord NVARCHAR(10),
@Longitude DECIMAL(18,10) OUTPUT,
@Latitude DECIMAL(18,10) OUTPUT
)
AS
EXTERNAL NAME Geographic.[UcbManagementInformation.Server.Utilities.GeographicUtility].ConvertNationalGridToLongLat

 */
namespace UcbManagementInformation.Server.Utilities
{

    public class GeographicUtility
    {
        [Microsoft.SqlServer.Server.SqlFunction]
        public static SqlDecimal ConvertNationalGridToLongitude(SqlString xCoord)
        {
            SqlDecimal SqlLongitude;
            SqlDecimal SqlLatitude;

            ConvertNationalGridToLongLat(xCoord, new SqlString("0"), out SqlLongitude, out SqlLatitude);
            return SqlLongitude;
        }

        [Microsoft.SqlServer.Server.SqlFunction]
        public static SqlDecimal ConvertNationalGridToLatitude(SqlString yCoord)
        {
            SqlDecimal SqlLongitude;
            SqlDecimal SqlLatitude;

            ConvertNationalGridToLongLat(new SqlString("0"), yCoord, out SqlLongitude, out SqlLatitude);
            return SqlLatitude;
        }
        [Microsoft.SqlServer.Server.SqlProcedure]
        public static void ConvertNationalGridToLongLat(SqlString xCoord, SqlString yCoord, out SqlDecimal SqlLongitude, out SqlDecimal SqlLatitude)
        {
            double LatidudeOut = 0;
            double LongitudeOut = 0;

            ConvertNationalGridToLongLat_internal(Convert.ToDouble(xCoord.Value), Convert.ToDouble(yCoord.Value), out LongitudeOut, out LatidudeOut);
            LatLong OSGB36 = new LatLong(LatidudeOut, LongitudeOut);

            LatLong WGS84LatLong = convertOSGB36toWGS84(OSGB36);

            SqlLatitude = (SqlDecimal)WGS84LatLong.Lat;
            SqlLongitude = (SqlDecimal)WGS84LatLong.Lon;

        }
        private static void ConvertNationalGridToLongLat_internal(double xCoord, double yCoord, out double Longitude, out double Latitude)
        {
            Latitude = 0;
            Longitude = 0;

            double E = xCoord;
            double N = yCoord;

            double a = 6377563.396;
            double b = 6356256.910;              // Airy 1830 major & minor semi-axes
            double F0 = 0.9996012717;                             // NatGrid scale factor on central meridian
            double lat0 = 49 * Math.PI / 180;
            double lon0 = -2 * Math.PI / 180;  // NatGrid true origin
            double N0 = -100000;
            double E0 = 400000;                     // northing & easting of true origin, metres

            double e2 = 1 - (b * b) / (a * a);                          // eccentricity squared
            double n = (a - b) / (a + b);
            double n2 = n * n;
            double n3 = n * n * n;

            Latitude = lat0;
            double M1 = 0;
            do
            {
                Latitude = (N - N0 - M1) / (a * F0) + Latitude;

                double Ma = (1 + n + (5 / 4) * n2 + (5 / 4) * n3) * (Latitude - lat0);
                double Mb = (3 * n + 3 * n * n + (21 / 8) * n3) * Math.Sin(Latitude - lat0) * Math.Cos(Latitude + lat0);
                double Mc = ((15 / 8) * n2 + (15 / 8) * n3) * Math.Sin(2 * (Latitude - lat0)) * Math.Cos(2 * (Latitude + lat0));
                double Md = (35 / 24) * n3 * Math.Sin(3 * (Latitude - lat0)) * Math.Cos(3 * (Latitude + lat0));
                M1 = b * F0 * (Ma - Mb + Mc - Md);             // meridional arc

            } while (N - N0 - M1 >= 0.00001);  // ie until < 0.01mm

            double cosLat = Math.Cos(Latitude);
            double sinLat = Math.Sin(Latitude);
            double nu = a * F0 / Math.Sqrt(1 - e2 * sinLat * sinLat);              // transverse radius of curvature
            double rho = a * F0 * (1 - e2) / Math.Pow(1 - e2 * sinLat * sinLat, 1.5);  // meridional radius of curvature
            double eta2 = nu / rho - 1;

            double tanLat = Math.Tan(Latitude);
            double tan2lat = tanLat * tanLat;
            double tan4lat = tan2lat * tan2lat;
            double tan6lat = tan4lat * tan2lat;
            double secLat = 1 / cosLat;
            double nu3 = nu * nu * nu;
            double nu5 = nu3 * nu * nu;
            double nu7 = nu5 * nu * nu;
            double VII = tanLat / (2 * rho * nu);
            double VIII = tanLat / (24 * rho * nu3) * (5 + 3 * tan2lat + eta2 - 9 * tan2lat * eta2);
            double IX = tanLat / (720 * rho * nu5) * (61 + 90 * tan2lat + 45 * tan4lat);
            double X = secLat / nu;
            double XI = secLat / (6 * nu3) * (nu / rho + 2 * tan2lat);
            double XII = secLat / (120 * nu5) * (5 + 28 * tan2lat + 24 * tan4lat);
            double XIIA = secLat / (5040 * nu7) * (61 + 662 * tan2lat + 1320 * tan4lat + 720 * tan6lat);

            double dE = (E - E0);
            double dE2 = dE * dE;
            double dE3 = dE2 * dE;
            double dE4 = dE2 * dE2;
            double dE5 = dE3 * dE2;
            double dE6 = dE4 * dE2;
            double dE7 = dE5 * dE2;
            Latitude = Latitude - VII * dE2 + VIII * dE4 - IX * dE6;
            Longitude = lon0 + X * dE - XI * dE3 + XII * dE5 - XIIA * dE7;

            Latitude = Latitude * 180 / Math.PI;
            Longitude = Longitude * 180 / Math.PI;
        }
        public interface IEllipse
        {
            double a { get; }
            double b { get; }
            double f { get; }
        }
        public class WGS84 : IEllipse
        {
            public const double A = 6378137;
            public const double B = 6356752.314;
            public const double F = 1 / 298.257223563;
            #region IEllipse Members

            public double a
            {
                get { return A; }
            }

            public double b
            {
                get { return B; }
            }

            public double f
            {
                get { return F; }
            }

            #endregion
        }
        public class Airy1830 : IEllipse
        {
            public const double A = 6377563.396;
            public const double B = 6356256.910;
            public const double F = 1 / 299.3249646;
            #region IEllipse Members

            public double a
            {
                get { return A; }
            }

            public double b
            {
                get { return B; }
            }

            public double f
            {
                get { return F; }
            }


            #endregion
        }
        public interface IHelmert
        {
            double tx { get; } double ty { get; }double tz { get; }  // m
            double rx { get; } double ry { get; } double rz { get; }  // sec
            double s { get; }
        }
        public class WGS84toOSGB36 : IHelmert
        {
            public const double TX = -446.448; public const double TY = 125.157; public const double TZ = -542.060;   // m
            public const double RX = -0.1502; public const double RY = -0.2470; public const double RZ = -0.8421;  // sec
            public const double S = 20.4894;

            #region IHelmert Members

            public double tx
            {
                get { return TX; }
            }

            public double ty
            {
                get { return TY; }
            }

            public double tz
            {
                get { return TZ; }
            }

            public double rx
            {
                get { return RX; }
            }

            public double ry
            {
                get { return RY; }
            }

            public double rz
            {
                get { return RZ; }
            }

            public double s
            {
                get { return S; }
            }

            #endregion
        }
        public class OSGB36toWGS84 : IHelmert
        {
            public const double TX = 446.448; public const double TY = -125.157; public const double TZ = 542.060;
            public const double RX = 0.1502; public const double RY = 0.2470; public const double RZ = 0.8421;
            public const double S = -20.4894;

            #region IHelmert Members

            public double tx
            {
                get { return TX; }
            }

            public double ty
            {
                get { return TY; }
            }

            public double tz
            {
                get { return TZ; }
            }

            public double rx
            {
                get { return RX; }
            }

            public double ry
            {
                get { return RY; }
            }

            public double rz
            {
                get { return RZ; }
            }

            public double s
            {
                get { return S; }
            }

            #endregion
        }
        public class LatLong
        {
            public LatLong(double latitude, double longitude)
                : this(latitude, longitude, 0)
            {

            }
            public LatLong(double latitude, double longitude, double height)
            {
                Lat = latitude;
                Lon = longitude;
                Height = height;
            }
            public double Lat;
            public double Lon;
            public double Height;
        }




        /*
                // ellipse parameters
            
    var e = { WGS84:    { a: 6378137,     b: 6356752.3142, f: 1/298.257223563 },
              Airy1830: { a: 6377563.396, b: 6356256.910,  f: 1/299.3249646   } };

    // helmert transform parameters
    var h = { WGS84toOSGB36: { tx: -446.448,  ty:  125.157,   tz: -542.060,   // m
                               rx:   -0.1502, ry:   -0.2470,  rz:   -0.8421,  // sec
                               s:    20.4894 },                               // ppm
              OSGB36toWGS84: { tx:  446.448,  ty: -125.157,   tz:  542.060,
                               rx:    0.1502, ry:    0.2470,  rz:    0.8421,
                               s:   -20.4894 } };
    */

        public static LatLong convertOSGB36toWGS84(LatLong p1)
        {
            LatLong p2 = convert(p1, new Airy1830(), new OSGB36toWGS84(), new WGS84());
            return p2;
        }

        /*
        function convertWGS84toOSGB36(p1) {
          var p2 = convert(p1, e.WGS84, h.WGS84toOSGB36, e.Airy1830);
          return p2;
        }
                */

        public static LatLong convert(LatLong p1, IEllipse e1, IHelmert t, IEllipse e2)
        {
            // -- convert polar to cartesian coordinates (using ellipse 1)

            //Conver to radians
            p1.Lat = p1.Lat * Math.PI / 180;
            p1.Lon = p1.Lon * Math.PI / 180;

            double a = e1.a;
            double b = e1.b;

            double sinPhi = Math.Sin(p1.Lat);
            double cosPhi = Math.Cos(p1.Lat);
            double sinLambda = Math.Sin(p1.Lon);
            double cosLambda = Math.Cos(p1.Lon);
            double H = p1.Height;

            double eSq = (a * a - b * b) / (a * a);
            double nu = a / Math.Sqrt(1 - eSq * sinPhi * sinPhi);

            double x1 = (nu + H) * cosPhi * cosLambda;
            double y1 = (nu + H) * cosPhi * sinLambda;
            double z1 = ((1 - eSq) * nu + H) * sinPhi;


            // -- apply helmert transform using appropriate params

            double tx = t.tx;
            double ty = t.ty;
            double tz = t.tz;
            double rx = t.rx / 3600 * Math.PI / 180;  // normalise seconds to radians
            double ry = t.ry / 3600 * Math.PI / 180;
            double rz = t.rz / 3600 * Math.PI / 180;
            double s1 = t.s / 1e6 + 1;              // normalise ppm to (s+1)

            // apply transform
            double x2 = tx + x1 * s1 - y1 * rz + z1 * ry;
            double y2 = ty + x1 * rz + y1 * s1 - z1 * rx;
            double z2 = tz - x1 * ry + y1 * rx + z1 * s1;


            // -- convert cartesian to polar coordinates (using ellipse 2)

            a = e2.a;
            b = e2.b;
            double precision = 4 / a;  // results accurate to around 4 metres

            eSq = (a * a - b * b) / (a * a);
            double p = Math.Sqrt(x2 * x2 + y2 * y2);
            double phi = Math.Atan2(z2, p * (1 - eSq)), phiP = 2 * Math.PI;
            while (Math.Abs(phi - phiP) > precision)
            {
                nu = a / Math.Sqrt(1 - eSq * Math.Sin(phi) * Math.Sin(phi));
                phiP = phi;
                phi = Math.Atan2(z2 + eSq * nu * Math.Sin(phi), p);
            }
            double lambda = Math.Atan2(y2, x2);
            H = p / Math.Cos(phi) - nu;

            return new LatLong(phi * 180 / Math.PI, lambda * 180 / Math.PI, H);
        }

    }
}
/* *********************************************************This Query is an example thatUpdates a table using a CURSOR and the CLR Stored Procedure***********************
 * DECLARE @Northing VARCHAR(10)
          , @EAsting VARCHAR(10)
,@Longitude DECIMAL(18,10),
@Latitude DECIMAL(18,10),
@Code UniqueIdentifier

    DECLARE TableCursor CURSOR  FAST_FORWARD FOR 
        SELECT Code,NORTHING, Easting
            FROM Organisation
WHERE Northing IS NOT  NULL AND EASTING IS NOT  NULL AND IsNumeric(Northing) = 1   And IsNumeric(Easting) =1  

    -- Open the cursor and fetch the first result
    OPEN TableCursor
    FETCH TableCursor INTO @Code, @Northing, @EAsting

    WHILE @@Fetch_status = 0  BEGIN
        
        
		EXECUTE [ConvertNationalGridToLongLat] 
		   @Easting
		  ,@Northing
		  ,@Longitude OUTPUT
		  ,@Latitude OUTPUT
				
		UPDATE Organisation
		SET Longitude = @longitude,Latitude=@latitude
		WHERE Organisation.Code = @Code

        FETCH TableCursor INTO @Code, @Northing, @Easting -- nxt Tbl
    END -- of the WHILE LOOP

    -- Clean up the cursor
    CLOSE TableCursor
    DEALLOCATE TableCursor
*/


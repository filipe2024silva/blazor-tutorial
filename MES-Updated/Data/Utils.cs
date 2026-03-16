using System.Security.Cryptography;
using System.Text;
using f10.pulsar.mes.data;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using MySqlConnector;

namespace f10.pulsar.mes
{

    public class Utils
    {

        public const string IP_PLC_L1 = "";
        public const int PORTA_PLC_L1 = 0;
        public const int DA1_L1 = 0;
        public const string IP_RS_L1 = "";
        public const int PORTA_RS_L1 = 0;

        public Dictionary<string, object> f10session;


        public bool L1ON;
        public bool MESON;
        public bool DarkTheme;
        public Syncfusion.Blazor.Theme ctheme;
        public string cthemetitle;
        public bool Refresh;

        public Utils()
        {
            f10session = new Dictionary<string, object>();
            L1ON = true;
            MESON = true;
            DarkTheme = false;
            Refresh = true;
        }

        public static string GeneratePass()
        {
            Random random = new Random();
            const string caps = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lcase = "abcdefghijklmnopqrstuvwxyz";
            const string nums = "0123456789";
            const string special = "!#@=-*&%$?";
            char[] pass = {
                caps[random.Next(caps.Length)],
                special[random.Next(special.Length)],
                lcase[random.Next(lcase.Length)],
                nums[random.Next(nums.Length)],
                nums[random.Next(nums.Length)],
                caps[random.Next(caps.Length)],
                lcase[random.Next(lcase.Length)],
                nums[random.Next(nums.Length)],
                special[random.Next(special.Length)],
                nums[random.Next(nums.Length)],
                nums[random.Next(nums.Length)],
                lcase[random.Next(lcase.Length)],
                nums[random.Next(nums.Length)]
            };
            string newpass = new string(pass);
            return newpass;
        }

        public string Hash(string s)
        {
            HashAlgorithm sha = SHA256.Create();
            byte[] result = sha.ComputeHash(Encoding.UTF8.GetBytes(s));
            string hashstring = BitConverter.ToString(result).Replace("-", "").ToLower();

            return hashstring;
        }

        public bool CanAccess(string tipo)
        {
            try
            {
                DataLink_MES dl = new DataLink_MES();
                dl.db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

                User user = (User)f10session["user"];

                if (user?.username == "f10")
                    return true;

                bool access = false;

                var perm = dl.db.Permissions.FirstOrDefault(x => x.userid == user.id && x.accessto == tipo);

                if (perm != null)
                    access = perm.allowed;

                var allPerm = dl.db.Permissions.FirstOrDefault(x => x.userid == user.id && x.accessto == "All");

                if (allPerm != null)
                    access = allPerm.allowed;

                dl.Dispose();
                return access;
            }
            catch (MySqlException)
            {
                return false;
            }
        }


        public async void UserLog(string coisas, string datalink)
        {
            User who = (User)f10session["user"];
            switch (datalink)
            {
                case "DataLink_MES":
                    DataLink_MES dl0 = new DataLink_MES();
                    dl0.db.UserLog.Add(new f10.pulsar.mes.data.Log { Username = who.name, Tempo = DateTime.Now, Comportamento = coisas, Nivel = "MES" });
                    await dl0.db.SaveChangesAsync();
                    dl0.Dispose();
                    break;
                case "DataLink_L1":
                    DataLink_L1 DL1 = new DataLink_L1();
                    DL1.db.Users.Add(new f10.pulsar.sv.data.User { Username = who.name, Tempo = DateTime.Now, Comportamento = coisas, Nivel = "MES" });
                    await DL1.db.SaveChangesAsync();
                    DL1.Dispose();
                    break;
                default:
                    break;
            }
        }

    }


    public class DataLink_MES : IDisposable
    {

        public MySqlDataContext db;

        public static string connStringMES =
#if DEBUG
        //pc local
        "Server=localhost;Port=3306;Database=desenvolvimento_mes;Uid=root;Pwd=2026;ConvertZeroDateTime=True";

#else
         "Server=localhost;Port=3306;Database=desenvolvimento_mes;Uid=f10;Pwd=fdecimal2015;ConvertZeroDateTime=True";
#endif

        public DataLink_MES(bool so_pra_ler = false)
        {
#if DEBUG

            var opb = new DbContextOptionsBuilder<MySqlDataContext>();
            opb.UseMySql(connStringMES, MariaDbServerVersion.AutoDetect(connStringMES));
            db = new MySqlDataContext(opb.Options);

            if (so_pra_ler)
                db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
#else
                    var opb = new DbContextOptionsBuilder<MySqlDataContext>();
            opb.UseMySql(connStringMES, MariaDbServerVersion.AutoDetect(connStringMES));
            db = new MySqlDataContext(opb.Options);
                    if (so_pra_ler)
                        db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
#endif

            if (db.Database.HasPendingModelChanges())
                db.Database.Migrate();
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }

    public class DataLink_L1 : IDisposable
    {
        public static string connStRL1 =
#if DEBUG
        //pc local
        "Server=localhost;Port=3306;Database=desenvolvimento_linha;Uid=root;Pwd=2026;ConvertZeroDateTime=True";

#else
        "Server=localhost;Port=3306;Database=desenvolvimento_linha;Uid=f10;Pwd=fdecimal2015;ConvertZeroDateTime=True";
#endif

        public pulsar.sv.data.mariadb.PulsarDataContext db;

        public DataLink_L1(bool so_pra_ler = false)
        {
#if DEBUG

            var opb = new DbContextOptionsBuilder<pulsar.sv.data.mariadb.PulsarDataContext>();
            opb.UseMySql(connStRL1, MariaDbServerVersion.AutoDetect(connStRL1));
            db = new pulsar.sv.data.mariadb.PulsarDataContext(opb.Options);
            if (so_pra_ler)
                db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            if (db.Database.HasPendingModelChanges())
                db.Database.Migrate();
#else
            var opb = new DbContextOptionsBuilder<pulsar.sv.data.mariadb.PulsarDataContext>();
            opb.UseMySql(connStRL1, MariaDbServerVersion.AutoDetect(connStRL1));
            db = new pulsar.sv.data.mariadb.PulsarDataContext(opb.Options);
            if (so_pra_ler)
                db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
#endif


        }
        public void Dispose()
        {
            db.Dispose();
        }
    }

    public static class FileUtil
    {
        public static ValueTask<object> SaveAs(this IJSRuntime js, string filename, byte[] data)
            => js.InvokeAsync<object>(
                "saveAsFile",
                filename,
                Convert.ToBase64String(data));
    }

    public class ThemeState
    {
        public event System.Action? OnThemeChanged;
        public void TriggerThemeChange()
        {
            OnThemeChanged?.Invoke();
        }
    }

}


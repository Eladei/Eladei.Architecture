namespace Eladei.BookInfo.Api.Helpers;

/// <summary>
/// Вспомогательный класс для работы с внешними переменными, хранящимися в .env-файле
/// </summary>
public static class EnvVariablesHelper {
    /// <summary>
    /// Запросить переменную из .env-файла
    /// </summary>
    /// <typeparam name="T">Тип переменной</typeparam>
    /// <param name="envVariableName">Название переменной</param>
    /// <returns>Переменная из .env-файла</returns>
    /// <exception cref="ArgumentNullException">Переменная не найдена</exception>
    /// <exception cref="NotImplementedException">Тип запрашиваемой переменной 
    /// не предусмотрен для запроса</exception>
    public static T GetVariable<T>(string envVariableName) {
        var envVariable = Environment.GetEnvironmentVariable(envVariableName) 
            ?? throw new ArgumentNullException(envVariableName);

        Type resultType = typeof(T);

        return resultType switch {
            Type t when t == typeof(string) => (T)(object)envVariable,
            Type t when t == typeof(int) => (T)(object)Convert.ToInt32(envVariable),
            Type t when t == typeof(bool) => (T)(object)Convert.ToBoolean(envVariable),
            Type t when t == typeof(double) => (T)(object)Convert.ToDouble(envVariable),
            Type t when t == typeof(float) => (T)(object)Convert.ToSingle(envVariable),
            Type t when t == typeof(long) => (T)(object)Convert.ToInt64(envVariable),
            Type t when t == typeof(short) => (T)(object)Convert.ToInt16(envVariable),
            Type t when t == typeof(decimal) => (T)(object)Convert.ToDecimal(envVariable),
            Type t when t == typeof(DateTime) => (T)(object)DateTime.Parse(envVariable),
            Type t when t == typeof(byte) => (T)(object)Convert.ToByte(envVariable),
            Type t when t == typeof(sbyte) => (T)(object)Convert.ToSByte(envVariable),
            Type t when t == typeof(char) => (T)(object)Convert.ToChar(envVariable),
            Type t when t == typeof(uint) => (T)(object)Convert.ToUInt32(envVariable),
            Type t when t == typeof(ulong) => (T)(object)Convert.ToUInt64(envVariable),
            Type t when t == typeof(ushort) => (T)(object)Convert.ToUInt16(envVariable),
            _ => throw new NotImplementedException()
        };
    }
}
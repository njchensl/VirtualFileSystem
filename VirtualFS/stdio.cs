using System.Runtime.InteropServices;

public static unsafe class stdio
{
    [DllImport("msvcrt.dll")]
    public static extern void* malloc(ulong size);

    [DllImport("msvcrt.dll")]
    public static extern void free(void* size);

    [DllImport("msvcrt.dll")]
    public static extern void* memcpy(void* destination, void* source, ulong num);

    [DllImport("msvcrt.dll")]
    public static extern ulong fread(void* buffer, ulong size, ulong count, void* stream);

    [DllImport("msvcrt.dll")]
    public static extern void* fopen(string filename, string filemode);

    [DllImport("msvcrt.dll")]
    public static extern int fclose(void* stream);

    [DllImport("msvcrt.dll")]
    public static extern ulong fwrite(void* ptr, ulong size, ulong nmemb, void* stream);
}
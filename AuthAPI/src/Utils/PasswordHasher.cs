using Isopoh.Cryptography.Argon2;

namespace AuthAPI.Utils
{
    public static class PasswordHasher
    {
        // Hashea y devuelve un string “codificado” con parámetros+sal+hash (todo incluido)
        public static string Hash(string plainPassword)
        {
            // Usa Argon2id por defecto con parámetros seguros del paquete
            // (si quieres personalizar costes, podemos hacerlo con Argon2Config)
            return Argon2.Hash(plainPassword);
        }

        // Verifica comparando la contraseña en plano con el hash almacenado
        public static bool Verify(string hashedPassword, string plainPassword)
        {
            return Argon2.Verify(hashedPassword, plainPassword);
        }
    }
}

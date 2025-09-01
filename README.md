# AuthAPI 🔐

API de autenticación desarrollada en **.NET 9** con **Entity Framework Core** y **MariaDB/MySQL**.  
Utiliza **Argon2** para el hash de contraseñas y **JWT** para la autenticación, con envío del token en **cookies HttpOnly**.

## 🚀 Tecnologías principales
- ASP.NET Core 9
- Entity Framework Core + Pomelo (MariaDB)
- Argon2 (Isopoh.Cryptography.Argon2)
- JWT (System.IdentityModel.Tokens.Jwt)
- Swagger para pruebas de la API

## ✨ Funcionalidades actuales
- Registro de usuarios con hash Argon2
- Login con validación segura de contraseñas
- Emisión de JWT firmados con HS256
- Almacenamiento del token en cookie HttpOnly
- Logout eliminando la cookie

## 📂 Estructura básica

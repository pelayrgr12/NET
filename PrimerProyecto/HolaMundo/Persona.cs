class Persona {
    public string nombre { get; set; }
    public int edad { get; set; }

    public Persona (string nombre, int edad) {
        this.nombre = nombre;
        this.edad = edad;
    }

    public void Saludar() {
        Console.WriteLine($"Hola, mi nombre es {nombre} y tengo {edad} años.");
    }

    public override string ToString(){
         return $"Persona: Nombre = {nombre}, Edad = {edad}"; 
    }
}

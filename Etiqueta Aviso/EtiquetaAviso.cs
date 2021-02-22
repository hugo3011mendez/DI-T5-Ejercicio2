using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Etiqueta_Aviso
{
    public enum eMarca // Defino el enumerado para los adornos
    {
        Nada,
        Cruz,
        Circulo,
        ImagenDeForma
    }


    public partial class EtiquetaAviso : Control // Hago un componente personalizado, dibujando en vez de metiendo componentes de Windows Forms
    {
        public EtiquetaAviso()
        {
            InitializeComponent();
        }

        [Category("Marca")]
        [Description("Indica que no se mostrará ninguna marca con la etiqueta")]
        private eMarca marca = eMarca.Nada; // En esta variable usaremos todos los posibles datos elegidos del enumerado
        // Aquí guardaremos el enumerado eMarca

        public eMarca Marca
        {
            set
            {
                marca = value;
                this.Refresh();
            }
            get { return marca; }
        }

        // Creo una variable para indicar la imagen que se dibujará si se escoge ImagenDeForma para la etiqueta aviso
        private Image imagenMarca;

        [Category("Marca")]
        [Description("Guarda la imagen que se muestra como marca cuando se escoge esta opción")]
        public Image ImagenMarca
        {
            set
            {
                imagenMarca = value;
            }

            get
            {
                return imagenMarca;
            }
        }


        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            Graphics g = pe.Graphics;
            int grosor = 0; //Grosor de las líneas de dibujo
            int offsetX = 0; //Desplazamiento a la derecha del texto
            int offsetY = 0; //Desplazamiento hacia abajo del texto

            //Esta propiedad provoca mejoras en la apariencia o en la eficiencia a la hora de dibujar
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            //Dependiendo del valor de la propiedad marca dibujamos una
            //Cruz o un Círculo
            switch (Marca)
            {
                case eMarca.Circulo:
                    grosor = 20;
                    g.DrawEllipse(new Pen(Color.Green, grosor), grosor, grosor, this.Font.Height, this.Font.Height);

                    offsetX = this.Font.Height + grosor;
                    offsetY = grosor;
                    break;


                case eMarca.Cruz:
                    grosor = 5;
                    Pen lapiz = new Pen(Color.Red, grosor);

                    g.DrawLine(lapiz, grosor, grosor, this.Font.Height, this.Font.Height);
                    g.DrawLine(lapiz, this.Font.Height, grosor, grosor, this.Font.Height);

                    offsetX = this.Font.Height + grosor;
                    offsetY = grosor / 2;
                    //Es recomendable liberar recursos de dibujo pues se pueden realizar muchos y cogen memoria

                    lapiz.Dispose();
                    break;


                case eMarca.ImagenDeForma: // Dibujo la imagen establecida en la variable imagenMarca
                    g.DrawImage(ImagenMarca, 0, 0, 40, 40);

                    // Y establezco los offsets para escribir el texto que se quiera
                    offsetX = this.Font.Height + 20;
                    offsetY = 10;
                    break;
            }

            // Finalmente pintamos el Texto desplazado si fuera necesario
            SolidBrush b = new SolidBrush(this.ForeColor);
            g.DrawString(this.Text, this.Font, b, offsetX + grosor, offsetY);

            Size tam = g.MeasureString(this.Text, this.Font).ToSize();
            this.Size = new Size(tam.Width + offsetX + grosor, tam.Height + offsetY * 2);

            b.Dispose();

            Graphics ge = pe.Graphics; //Creo otro objeto graphics para enseñar las transformaciones

            // Traslación
            ge.TranslateTransform(100, 100);
            ge.DrawLine(Pens.Red, 0, 0, 100, 0);
            ge.ResetTransform();

            //// Rotación de 30o en sentido horario
            //ge.RotateTransform(30);
            //ge.DrawLine(Pens.Blue, 0, 0, 100, 0);
            //ge.ResetTransform();

            // Traslación + rotación
            ge.TranslateTransform(100, 100);
            ge.RotateTransform(30);
            ge.DrawLine(Pens.Green, 0, 0, 100, 0);
            ge.ResetTransform();
        }


        protected override void OnTextChanged(EventArgs e) //Creamos un On para cada vez que se lanza el evento TextChanged
        {
            base.OnTextChanged(e);
            this.Refresh(); // Y lanzo Refresh() para que se dibuje con todo
        }

        [Category("Fondo")]
        [Description("Indica si se dibuja el gradiente de fondo o no")]
        private bool gradiente = false;
        public bool Gradiente
        {
            set
            {
                gradiente = value;
            }

            get
            {
                return gradiente;
            }
        }


        [Category("Fondo")]
        [Description("Es el primer color que influye en el gradiente al formarse")]
        private Color color1Gradiente;
        public Color Color1Gradiente
        {
            set
            {
                color1Gradiente = value;
            }

            get
            {
                return color1Gradiente;
            }
        }


        [Category("Fondo")]
        [Description("Es el segundo color que influye en el gradiente al formarse")]
        private Color color2Gradiente;
        public Color Color2Gradiente
        {
            set
            {
                color2Gradiente = value;
            }

            get
            {
                return color2Gradiente;
            }
        }


        [Category("Eventos")]
        [Description("Se lanza cuando se hace click sobre la marca, si existe")]
        public event EventHandler ClickEnMarca;
    }
}

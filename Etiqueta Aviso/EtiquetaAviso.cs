using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
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
        int xMarca, yMarca; // Para conseguir las coordenadas máximas de la marca en cuestión

        int grosor; //Grosor de las líneas de dibujo
        int offsetX; //Desplazamiento a la derecha del texto
        int offsetY; //Desplazamiento hacia abajo del texto

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

            //Esta propiedad provoca mejoras en la apariencia o en la eficiencia a la hora de dibujar
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;


            if (Gradiente && Color1Gradiente != null && Color2Gradiente != null)
            {
                LinearGradientBrush gradientColor = new LinearGradientBrush(
                    new PointF(0, 0),
                    new PointF(this.Width, this.Height),
                    Color1Gradiente,
                    Color2Gradiente);

                g.FillRectangle(gradientColor, new RectangleF(
                    0, 0,
                    this.Width, this.Height));
            }


            //Dependiendo del valor de la propiedad marca dibujamos una
            //Cruz o un Círculo
            switch (Marca)
            {
                case eMarca.Circulo:
                    grosor = 20;

                    // Actualizo el valor de las coordenadas máximas de la marca
                    xMarca = grosor;
                    yMarca = grosor;

                    g.DrawEllipse(new Pen(Color.Green, grosor), grosor, grosor, this.Font.Height, this.Font.Height);

                    offsetX = this.Font.Height + grosor;
                    offsetY = grosor;
                    break;


                case eMarca.Cruz:
                    grosor = 5;
                    Pen lapiz = new Pen(Color.Red, grosor);

                    // Actualizo el valor de las coordenadas máximas de la marca
                    xMarca = this.Font.Height;
                    yMarca = this.Font.Height;

                    g.DrawLine(lapiz, grosor, grosor, this.Font.Height, this.Font.Height);
                    g.DrawLine(lapiz, this.Font.Height, grosor, grosor, this.Font.Height);

                    offsetX = this.Font.Height + grosor;
                    offsetY = grosor / 2;
                    //Es recomendable liberar recursos de dibujo pues se pueden realizar muchos y cogen memoria

                    lapiz.Dispose();
                    break;


                case eMarca.ImagenDeForma: // Dibujo la imagen establecida en la variable imagenMarca

                    if (ImagenMarca != null)
                    {
                        try
                        {
                            // Actualizo el valor de las coordenadas máximas de la marca
                            xMarca = 40;
                            yMarca = 40;

                            g.DrawImage(ImagenMarca, 0, 0, 40, 40);

                            // Y establezco los offsets para escribir el texto que se quiera
                            offsetX = this.Font.Height + 20;
                            offsetY = 10;
                        }
                        catch (ArgumentException)
                        {
                            Marca = eMarca.Nada;
                        }
                    }
                    else
                    {
                        Marca = eMarca.Nada;
                    }

                    break;


                case eMarca.Nada:
                    // Pongo los valores a 0 cuando es Nada
                    offsetX = 0;
                    offsetY = 0;
                    grosor = 0;

                    break;
            }


            // Finalmente pintamos el Texto desplazado si fuera necesario
            SolidBrush b = new SolidBrush(this.ForeColor);
            g.DrawString(this.Text, this.Font, b, offsetX + grosor, offsetY);

            Size tam = g.MeasureString(this.Text, this.Font).ToSize();
            this.Size = new Size(tam.Width + offsetX + grosor, tam.Height + offsetY * 2);

            b.Dispose();
        }


        protected override void OnTextChanged(EventArgs e) //Creamos un On para cada vez que se lanza el evento TextChanged
        {
            base.OnTextChanged(e);
            this.Refresh(); // Y lanzo Refresh() para que se dibuje con todo
        }

        private bool gradiente = false;
        [Category("Fondo")]
        [Description("Indica si se dibuja el gradiente de fondo o no")]
        public bool Gradiente
        {
            set
            {
                gradiente = value;
                this.Refresh();
            }

            get
            {
                return gradiente;
            }
        }


        private Color color1Gradiente;
        [Category("Fondo")]
        [Description("Es el primer color que influye en el gradiente al formarse")]
        public Color Color1Gradiente
        {
            set
            {
                color1Gradiente = value;
                this.Refresh();
            }

            get
            {
                return color1Gradiente;
            }
        }


        private Color color2Gradiente;
        [Category("Fondo")]
        [Description("Es el segundo color que influye en el gradiente al formarse")]
        public Color Color2Gradiente
        {
            set
            {
                color2Gradiente = value;
                this.Refresh();
            }

            get
            {
                return color2Gradiente;
            }
        }


        [Category("Eventos")]
        [Description("Se lanza cuando se hace click sobre la marca, si existe")]
        public event EventHandler ClickEnMarca;

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (Marca != eMarca.Nada)
            {
                if (e.X <= xMarca && e.Y <= yMarca)
                {
                    ClickEnMarca?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}

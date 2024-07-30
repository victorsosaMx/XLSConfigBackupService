using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceProcess;
using System.Text;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;




namespace XLSConfigBackupService
{
    public partial class XLSBackupService : ServiceBase
    {
        #region Variables
        Timer oTimer;
        
        bool ProgramaPrincipalEjecutandose = false;
        //readonly string DirectorioLog_Demonio = ConfigurationManager.AppSettings["DirectorioLog_Demonio"].ToString();
        readonly int Demonio_TiempoEsperaEnSegundos = int.Parse(ConfigurationManager.AppSettings["Demonio_TiempoEsperaEnSegundos"].ToString());
        readonly int Demonio_TiempoEsperaAlEscribirLogMilisegundos = int.Parse(ConfigurationManager.AppSettings["Demonio_TiempoEsperaAlEscribirLogMilisegundos"].ToString());
        readonly string Demonio_HoraInicioActividad = ConfigurationManager.AppSettings["Demonio_HoraInicioActividad"].ToString();
        readonly string Demonio_HoraFinActividad = ConfigurationManager.AppSettings["Demonio_HoraFinActividad"].ToString();
        private Dictionary<string, string> applications;
        readonly string DirectorioBackup = ConfigurationManager.AppSettings["DirectorioBackup"].ToString();


        #endregion

        #region "Parametros y funciones de programa"
        public XLSBackupService()
        {
            InitializeComponent();
            LoadApplicationSettings();
        }

        protected override void OnStart(string[] args)
        {
            ProgramaPrincipal();
            TimerCallback oCallBack = new TimerCallback(ProcessTimerEvent);
            oTimer = new Timer(oCallBack, null, new TimeSpan(0, 0, 0, Demonio_TiempoEsperaEnSegundos, 0), new TimeSpan(0, 0, 0, Demonio_TiempoEsperaEnSegundos, 0));
        }

        protected override void OnStop()
        {
            oTimer.Dispose();
        }

        public void Debug()
        {
            OnStart(null);
        }

        private void ProcessTimerEvent(object obj)
        {
            if (!ProgramaPrincipalEjecutandose)
                ProgramaPrincipal();
        }




        /// <summary>
        /// Método principal del programa.
        /// </summary>
        private void ProgramaPrincipal()
        {
            // Verifica si se permite ejecutar el demonio.
            // Si no se permite, se sale del método.
            if (!PermitirEjecutarDemonio())
                return;

            // Verifica si el programa principal ya está en ejecución.
            // Si no está en ejecución, se inicia.
            if (!ProgramaPrincipalEjecutandose)
            {
                ProgramaPrincipalEjecutandose = true;

                string MensajeLog;

                try
                {
                    // Intenta copiar archivos.
                    CopiarArchivos();

                }
                catch (Exception ex)
                {
                    /****************************************************************
                     * En caso de un error de comunicación a la base de datos, solo se 
                     * forzará a dejar un log en el EventViewer para que se pueda atender,
                     * pero el demonio debe de continuar ejecutandose
                     * **************************************************************/

                    // Crea un mensaje de error con la excepción y su excepción interna, si existe.
                    string MensajeError = "Exception: " + ex.Message;
                    if (ex.InnerException != null)
                        MensajeError += " Inner Exception: " + ex.InnerException.Message;

                    // Registra el error en el visor de eventos.
                    //clsUtilerias.RegistrarVisorEventos("Error al momento de ......: " + MensajeError, EventLogEntryType.Error);
                }

                // Indica que el programa principal ha terminado su ejecución.
                ProgramaPrincipalEjecutandose = false;
            }
        }

        private bool PermitirEjecutarDemonio()
        {
            /*********************************************************
             * Revisamos si el momento justo de ejecución esta fuera del
             * rango de periodo en el que debe de permanecer activo
             * *****************************************************/
            TimeSpan HoraInicioActividad = TimeSpan.Parse(Demonio_HoraInicioActividad);
            TimeSpan HoraFinActividad = TimeSpan.Parse(Demonio_HoraFinActividad);
            TimeSpan HoraActual = DateTime.Now.TimeOfDay;
            bool PermitirEjecutar = false;
            PermitirEjecutar = HoraInicioActividad < HoraActual && HoraActual < HoraFinActividad;
            return PermitirEjecutar;
        }

        private void GenerarLogResultado(string Mensaje)
        {
            //string PathAbsolutoDirectorio = Path.Combine(
            //  DirectorioLog_Demonio, //Directorio que indica el tipo de Log a generar (Json o Servicio)
            //  DateTime.Today.Year.ToString(), //Año de ejecución del servicio
            //  DateTime.Today.Month.ToString("00"), //Mes de ejecución del servicio
            //  DateTime.Today.Day.ToString("00")); //Dia de ejecución del servicio

            //if (!Directory.Exists(PathAbsolutoDirectorio))
            //    Directory.CreateDirectory(PathAbsolutoDirectorio);

            //string Contenido = $"[{DateTime.Now.ToString("HH:mm:ss.fff")}]\t\tMensaje: {Mensaje}";
            //string PathAbsolutoArchivo = Path.Combine(PathAbsolutoDirectorio, "Log_DemonioXLSBackupService.txt");
            //using (FileStream fs = new FileStream(PathAbsolutoArchivo, FileMode.Append, FileAccess.Write))
            //{
            //    using (StreamWriter fw = new StreamWriter(fs))
            //    {
            //        fw.WriteLine(Contenido);
            //        fw.Close();
            //        fw.Dispose();
            //    }
            //    fs.Close();
            //    fs.Dispose();
            //}
            //Thread.Sleep(Demonio_TiempoEsperaAlEscribirLogMilisegundos);
        }
        #endregion

        #region "Programa Principal"
        private void CopiarArchivos()
        {
            string backupBasePath = DirectorioBackup;
            DateTime currentDate = DateTime.Now;
            string month = currentDate.ToString("MM");
            string day = currentDate.ToString("dd");

            string[] configFiles = { "web.config", "app.config", "appsettings.json" };

            foreach (var app in applications)
            {
                string appPath = app.Value;
                string backupPath = Path.Combine(backupBasePath, app.Key, month, day);
                Directory.CreateDirectory(backupPath);

                foreach (string configFile in configFiles)
                {
                    string sourceFile = Path.Combine(appPath, configFile);
                    if (File.Exists(sourceFile))
                    {
                        string destinationFile = Path.Combine(backupPath, configFile);
                        File.Copy(sourceFile, destinationFile, true);
                    }
                }
            }

        }



        private void LoadApplicationSettings()
        {
            applications = new Dictionary<string, string>();

            foreach (string key in ConfigurationManager.AppSettings)
            {
                if (key.Contains("App__")) {
                    string value = ConfigurationManager.AppSettings[key];
                    string NewKey = key.ToString().Replace("App__", "");
                    applications.Add(NewKey, value);
                }                
            }
        }



        #endregion



    }

}
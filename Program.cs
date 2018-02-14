using DemoBehavior.DemoSii;
using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Net;
using System.IO;

namespace DemoBehavior
{
	class Program
	{
		static void Main(string[] args)
		{
			string s;

			SuministroLRFacturasEmitidas sumi = new SuministroLRFacturasEmitidas();
			LRfacturasEmitidasType lr = new LRfacturasEmitidasType();
			FacturaExpedidaType factexp = new FacturaExpedidaType();

			PersonaFisicaJuridicaESType pers = new PersonaFisicaJuridicaESType();
			pers.NIF = "00000000T";
			pers.NombreRazon = "EMPRESA QUE EMITE FACTURAS";

			CabeceraSii cabe = new CabeceraSii();
			cabe.Titular = pers;
			sumi.Cabecera = cabe;

			RegistroSiiPeriodoImpositivo perio = new RegistroSiiPeriodoImpositivo();
			perio.Ejercicio = "2018";
			perio.Periodo = TipoPeriodoType.Item03; // Febrero
			lr.PeriodoImpositivo = perio;

			IDFacturaExpedidaTypeIDEmisorFactura emis = new IDFacturaExpedidaTypeIDEmisorFactura();
			emis.NIF = pers.NIF;
			IDFacturaExpedidaType idExp = new IDFacturaExpedidaType();
			idExp.IDEmisorFactura = emis;
			idExp.NumSerieFacturaEmisor = "Linda Factura Demo";
			idExp.FechaExpedicionFacturaEmisor = "08-02-2018";
			lr.IDFactura = idExp;

			PersonaFisicaJuridicaType contr = new PersonaFisicaJuridicaType();
			contr.NombreRazon = "Amazon.co.uk";
			contr.Item = "N1081152I";
			factexp.Contraparte = contr;
			factexp.TipoFactura = ClaveTipoFacturaType.F1;
			factexp.FechaOperacion = idExp.FechaExpedicionFacturaEmisor;
			factexp.ClaveRegimenEspecialOTrascendencia = IdOperacionesTrascendenciaTributariaType.Item01;
			factexp.DescripcionOperacion = "Mi maravillosa descripción"; // <element name="DescripcionOperacion" type="sii:TextMax500Type"/>

			FacturaExpedidaTypeTipoDesglose desgl = new FacturaExpedidaTypeTipoDesglose();
			TipoConDesgloseType condesgl = new TipoConDesgloseType();
			TipoSinDesglosePrestacionType pr = new TipoSinDesglosePrestacionType();
			SujetaPrestacionType sj = new SujetaPrestacionType();
			SujetaPrestacionTypeNoExenta nx = new SujetaPrestacionTypeNoExenta();
			DetalleIVAEmitidaPrestacionType dtp;
			dtp = new DetalleIVAEmitidaPrestacionType();
			dtp.BaseImponible = "667.00";
			dtp.CuotaRepercutida = "66.70";
			dtp.TipoImpositivo = "10.00";
			nx.DesgloseIVA = new DetalleIVAEmitidaPrestacionType[1];
			nx.DesgloseIVA.SetValue(dtp, 0);
			nx.TipoNoExenta = TipoOperacionSujetaNoExentaType.S1;
			sj.NoExenta = nx;
			pr.Sujeta = sj;
			condesgl.PrestacionServicios = pr;
			desgl.Item = condesgl;
			factexp.TipoDesglose = desgl;
			factexp.ImporteTotal = "733.70";

			lr.FacturaExpedida = factexp;
			sumi.RegistroLRFacturasEmitidas = new LRfacturasEmitidasType[1];
			sumi.RegistroLRFacturasEmitidas.SetValue(lr, 0);

			// A enviar ...
			RespuestaLRFEmitidasType resp;
			siiSOAPClient clsSiiService;
			try
			{
				clsSiiService = new siiSOAPClient("SuministroFactEmitidasPruebas");
				X509Certificate2 oCertificado = SeleccionarCertificado("Seleccione certificado", StoreLocation.CurrentUser, StoreName.My);
				clsSiiService.ClientCredentials.ClientCertificate.Certificate = oCertificado;
				clsSiiService.Open();
				if (clsSiiService.State == System.ServiceModel.CommunicationState.Opened)
				{
					resp = clsSiiService.SuministroLRFacturasEmitidas(sumi);
				}
			}
			catch (ProtocolException ex)
			{
				s = ex.Message;
				Console.Write(s); // throw ex;
			}
			catch (WebException ex) //catch (Exception ex)
			{
				var st = new StreamReader(ex.Response.GetResponseStream());
				s = st.ReadToEnd();
				Console.Write(s); // throw ex;
			}
			finally
			{
				clsSiiService = null;
			}
		} // Main

		private static X509Certificate2 SeleccionarCertificado(string vMessage, StoreLocation vLocation, StoreName vName)
		{
			X509Store certStore;
			X509Certificate2 oCertificado = null;
			try
			{
				certStore = new X509Store(vName, vLocation);
				certStore.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
				X509Certificate2Collection certsSelect = certStore.Certificates.Find(X509FindType.FindByTimeValid, DateTime.Now, true);
				X509Certificate2Collection certs =
					X509Certificate2UI.SelectFromCollection(
						certsSelect,
						"Selección certificado",
						vMessage,
						X509SelectionFlag.SingleSelection, IntPtr.Zero);
				if (certs.Count > 0)
				{    //Almacenamos el certificado en nuestro objeto interno
					oCertificado = certs[0];
				}
				certStore.Close();
				return oCertificado;
			}
			catch (Exception e)
			{
				throw (e);
			}
			finally
			{
				certStore = null;
			}
		} //Obtiene un certificado de usuario

	} // class
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNet3.AppLib.Services
{
    public class ServiceRequest
    {
        // Reference oluşturulurken adreslerin http olarak oluşturulması sebebi ile bu iptal:

        // MEYES.MeyesIntraSoapClient client = new MEYES.MeyesIntraSoapClient(MEYES.MeyesIntraSoapClient.EndpointConfiguration.MeyesIntraSoap);



        ////BasicHttpsBinding meyesBinding = new BasicHttpsBinding();

        ////meyesBinding.MaxBufferSize = int.MaxValue;

        ////    meyesBinding.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;

        ////    meyesBinding.MaxReceivedMessageSize = int.MaxValue;

        ////    meyesBinding.AllowCookies = true;

 

        ////    // https://netintratest.sgk.intra/WS_MeyesIntra/MeyesIntra.asmx

        ////    // https://netintraws.sgk.intra/WS_MeyesIntra/MeyesIntra.asmx

 

        ////    EndpointAddress meyesTestAddress = new EndpointAddress(_dataConfig.MeyesNetTest);

        ////EndpointAddress meyesProdAddress = new EndpointAddress(_dataConfig.MeyesNetProd);



        ////EndpointAddress meyesAddress = typeof(MEYES.MeyesIntraSoapClient).Namespace == "MeyesIntraTest" ? meyesTestAddress : meyesProdAddress;



        ////MEYES.MeyesIntraSoapClient client = new MEYES.MeyesIntraSoapClient(meyesBinding, meyesTestAddress);
    }
}

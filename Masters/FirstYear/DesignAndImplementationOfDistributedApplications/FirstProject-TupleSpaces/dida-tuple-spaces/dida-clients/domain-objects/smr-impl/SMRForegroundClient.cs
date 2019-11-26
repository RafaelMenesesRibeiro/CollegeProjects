using dida_clients.helpers;
using dida_contracts.domain_objects;

namespace dida_clients.domain_objects.smr_impl
{
    class SMRForegroundClient : ForegroundClient
    {
        public SMRForegroundClient(SMRBackgroundClient backgroundClient) : base(backgroundClient) { }

        public override void Write(DIDATuple tuple)
        {
            backgroundClient.Write(tuple);
        }

        public override void Read(DIDATuple tuple)
        {
            backgroundClient.Read(tuple);
        }

        public override void Take(DIDATuple tuple)
        {
            backgroundClient.Take(tuple);
        }
    }
}
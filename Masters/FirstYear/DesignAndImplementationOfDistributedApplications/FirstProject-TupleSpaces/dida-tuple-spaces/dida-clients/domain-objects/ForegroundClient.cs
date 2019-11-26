using dida_contracts.domain_objects;

namespace dida_clients.domain_objects
{
    public abstract class ForegroundClient
    {
        protected BackgroundClient backgroundClient;

        public void SetVerbose(bool boolean)
        {
            backgroundClient.Verbose = boolean;
        }

        public ForegroundClient(BackgroundClient backgroundClient)
        {
            this.backgroundClient = backgroundClient;
        }

        public virtual void Write(DIDATuple tuple)
        {
            backgroundClient.Write(tuple);
        }

        public virtual void Read(DIDATuple template)
        {
            backgroundClient.Read(template);
        }

        public virtual void Take(DIDATuple template)
        {
            backgroundClient.Take(template);
        }
    }
}

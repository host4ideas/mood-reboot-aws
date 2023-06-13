using Amazon;
using Amazon.IVS;
using Amazon.IVS.Model;

namespace MoodReboot.Services
{
    public class ServiceIVS
    {
        private readonly AmazonIVSClient _ivsClient;

        public ServiceIVS()
        {
            _ivsClient = new AmazonIVSClient("AKIAX3TY4MFICEYXAQ36", "Dw7UszDGyAOoJz5LYWEjm0ZNzMyZmafbi8+gw3zB", RegionEndpoint.USEast1);
        }


        //CREA UN CANAL
        public async Task<string> CreateChannel(string channelName)
        {
            CreateChannelResponse response = await _ivsClient.CreateChannelAsync(new CreateChannelRequest
            {
                Name = channelName
            });

            return response.Channel.Arn;
        }

        //BUSCA EL CANAL POR NOMBRE
        public async Task<Channel> GetChannelByName(string channelName)
        {
            ListChannelsResponse response = await _ivsClient.ListChannelsAsync(new ListChannelsRequest
            {
                MaxResults = 50 // Ajusta el número máximo de resultados según tus necesidades
            });

            ChannelSummary channelSummary = response.Channels.FirstOrDefault(channel => channel.Name.Equals(channelName, StringComparison.OrdinalIgnoreCase));

            if (channelSummary != null)
            {
                GetChannelResponse getChannelResponse = await _ivsClient.GetChannelAsync(new GetChannelRequest
                {
                    Arn = channelSummary.Arn
                });

                return getChannelResponse.Channel;
            }

            return null;
        }

        //CONSIGUE EL STREAM ARN DE UN CANAL POR SU ARN
        public async Task<string> GetStreamKeyFromChannelArn(string channelArn)
        {
            string region = channelArn.Split(':')[3]; // Extraer la región del ARN
            string channelId = channelArn.Split('/')[1]; // Extraer el ID del canal del ARN

            using (var ivsClient = new AmazonIVSClient(RegionEndpoint.GetBySystemName(region)))
            {
                ListStreamKeysResponse response = await ivsClient.ListStreamKeysAsync(new ListStreamKeysRequest
                {
                    ChannelArn = channelArn
                });

                string streamKey = response.StreamKeys.FirstOrDefault().Arn;

                return streamKey;
            }
        }

        //CONSIGUE LA KEY DE RETRANSMISION A PARTIR DEL STREAM ARN
        public async Task<string> GetStreamKey(string streamKeyArn)
        {
            using (var ivsClient = new AmazonIVSClient(RegionEndpoint.USEast1))
            {
                GetStreamKeyResponse response = await ivsClient.GetStreamKeyAsync(new GetStreamKeyRequest
                {
                    Arn = streamKeyArn
                });

                return response.StreamKey.Value;
            }
        }

        //MIRA SI ESTA EN DIRECTO O NO
        public async Task<Amazon.IVS.Model.Stream> IsLive(string channelArn)
        {
            try
            {
                var getStreamRequest = new GetStreamRequest
                {
                    ChannelArn = channelArn
                };

                var getStreamResponse = await _ivsClient.GetStreamAsync(getStreamRequest);

                return getStreamResponse.Stream;
            }
            catch (Exception ex)
            {
                //no esta online
                return null;
            }
        }




    }
}

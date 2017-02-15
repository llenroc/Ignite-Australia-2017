#r "Newtonsoft.Json"

using System;
using System.Net;
using Newtonsoft.Json;

public static object Run(HttpRequestMessage req, out Voting outputEventHubMessage, TraceWriter log)
{
    log.Info($"Webhook was triggered!");

    string jsonContent = req.Content.ReadAsStringAsync().Result;
    dynamic data = JsonConvert.DeserializeObject(jsonContent);
    log.Info(jsonContent);

    if (data.text != null) {
        string text = data.text;
        var voteResponse = new Voting{
            user = data.user,
            text = data.text,
            vote = "",
            location = data.location
        };

        if (text.ToLower().Contains("tabs") && text.ToLower().Contains("spaces")){
            voteResponse.vote = "-_-";
            outputEventHubMessage = voteResponse;
            return req.CreateResponse(HttpStatusCode.OK, new {
                vote = voteResponse.vote
            });
        }

        else if (text.ToLower().Contains("tabs")){
            voteResponse.vote = "tabs";
            outputEventHubMessage = voteResponse;
            return req.CreateResponse(HttpStatusCode.OK, new {
                vote = voteResponse.vote
            });
        }

        else if (text.ToLower().Contains("spaces")){
            voteResponse.vote = "spaces";
            outputEventHubMessage = voteResponse;
            return req.CreateResponse(HttpStatusCode.OK, new {
                vote = voteResponse.vote
            });
        }

        else{
            outputEventHubMessage = new Voting() { vote = $"null" };
            return req.CreateResponse(HttpStatusCode.OK);
        }
    }

    outputEventHubMessage = new Voting() { vote = $"null" };
    return req.CreateResponse(HttpStatusCode.OK);
}

public class Voting{
    public string user {get; set;}
    public string text {get; set;}
    public string vote {get; set;}
    public string location {get; set;} 
}
const Crypto = require('crypto');

module.exports = async function (context, req) {
    context.log('JavaScript HTTP trigger function processed a request.');

    const hmac = Crypto.createHmac("sha1", "{shhh...ThisIsASecret}");
    const signature = hmac.update(JSON.stringify(req.body)).digest('hex');
    const shaSignature = `sha1=${signature}`;

    const gitHubSignature = req.headers['x-hub-signature'];

    const name = (req.query.name || (req.body && req.body.name));
    const responseMessage = name
        ? "Hello, " + name + ". This HTTP triggered function executed successfully."
        : "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response.";

    if(!shaSignature.localeCompare(gitHubSignature)){
        if (req.body.pages[0].title){
            context.res = {
                body: "Event Type is : " + req.headers['x-github-event'] + "\n" + req.body.sender.login + ", has " + req.body.pages[0].action + " in Page " + req.body.pages[0].title
            };
        }
        else {
            context.res = {
                status: 400,
                body: ("Invalid payload for Wiki event")
            };
        }
    }
    else { 
        context.res = {
            status: 401,
            body: "Signature Doesn't Match"
        };
    }
}

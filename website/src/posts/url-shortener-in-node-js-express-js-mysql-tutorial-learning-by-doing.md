---
title: URL shortener in Node.js + Express.js + MySQL tutorial: learning by doing
date: 2015-02-11T21:40:03+02:00
description: I made a URL shortener not that long ago, which uses Node.js, Express.js and a MySQL database as backend. I thought it would be fun for my first real tutorial to explain how to make such a simple application like a URL shortener.
---

**[Download the source code here](https://github.com/dukeofharen/urlshortener.js).**

Node.js brings the power of JavaScript to the server. Its power lies in the fact that it has out-of-the-box support for WebSockets (realtime connections with the client application in the browser), so that it looks like a client app has real push message support, and that it can have many simultaneous connections. The Node package manager (npm for short) contains many production ready modules, under which Express.js, which you can easily install through the command line. Express.js is a framework built upon Node.js which handles a lot of stuff, like fetching POST data, session handling, serving resources (CSS, JS files for the client app), routing of URLs and templates.

I made a URL shortener not that long ago, which uses Node.js, Express.js and a MySQL database as backend. I thought it would be fun for my first real tutorial to explain how to make such a simple application like a URL shortener.

## URL shortener?

I imagine that you know what a URL shortener is, but I'll explain anyway. Let's say we have a URL: ridiculouslylongurlimeanit.com/veryniceblogpost.html. This is a horrible URL to share on Twitter or even on a forum. It would be a lot nicer to have this URL shortened: e.g. d.co/1sk59. This is a nicer URL to share.

## What should it do?

**For the information, this tutorial is only focused on creating an API for creating shortened URLs. If you want the app to have a graphical user interface, you have to implement it yourself. It isn't very hard to create a GUI using Express.js and e.g. the Handlebars template parser. Maybe a theme for a next tutorial :) Also, a basic knowledge of JavaScript and programming isn't going to hurt you in this tutorial.**

Some nice functionalities for a personal URL shortener:

*   Shortening of URLs (duh)
*   Simple statistics (e.g. which IP address clicked it, what is the referring web page)
*   Possibility of creating custom URLs (d.co/customurl instead of d.co/1wd9fg)

I'm going to implement and explain these topics.

## Before we begin

*[Download and install Node.js](http://nodejs.org/download/)* and *[MySQL](http://www.mysql.com/)* (if you haven't done so already). This installs Node.js on your system so you can start a Node app from the command line. Second, I recommend installing [Nodemon](https://www.npmjs.com/package/nodemon) (I always use it). This is a Node.js module which essentially "monitors" the files of your Node.js application and restarts it if a JS file is changed. In contrast to for example PHP, changes in .js files aren't picked up until the Node app is restarted.

To install nodemon, execute `npm install -g nodemon` through the command line. The "-g" means "install this app globally", so it can also be used for other Node.js apps. If you leave out the "-g", the module will be installed in the folder "node_modules" in the folder of your Node app.

## Basic setup: let's get started

One of the most important files in a Node.js application is the "package.json" file. This is a file which contains information about the app, which dependencies it has, how the app should be started etc. This is our package.json file:

    {
        "name": "urlshortener",
        "description": "Node.js URL shortener",
        "version": "0.0.1",
        "dependencies": {
            "body-parser": "^1.7.0",
            "express": "3.x",
            "mysql": "*",
            "request": "*"
        }
    }


What does it say? The name, description and version are pretty obvious. The "dependencies" maybe not. The names on the left are the name of the modules which are needed for the application to run. The string on the right is the version of the module which should be used. `*` means "any version". `^1.7.0` means "any version from 1.7.0 to 2.0.0". `3.x` means "any version from 3.0 to 4.0". The module name is the name of the module on <https://www.npmjs.com/>. To install these modules, open a command line, browse to the folder where you are going to develop the URL shortener and execute `npm install`. This command starts npm and checks the dependencies in the "package.json" file and installs them in the "node_modules" folder in that folder. Here is a quick explanation of the modules:

*   body-parser: this module is used in combination with Express.js to easily parse POST data sent to the Node application.
*   express: the Express.js framework
*   mysql: as the name says, a Node.js connector for the MySQL database
*   request: a straightforward module to make requests to another application (e.g. to check if a URL a user has submitted actually exists)

For more information about the package file, visit <http://browsenpm.org/package.json>.

Before getting started with the actual application, let's create a data structure. This is what we're going to store in the MySQL database. The database contains 2 tables:

**urls**: the actual URL table  
*id*: the unique ID of the short url  
*url*: the long URL  
*segment*: the unique short string for the URL (e.g. 1djf8)  
*datetime_added*: the date and time the URL is added  
*ip*: the IP address of the person who added the URL  
*num_of_clicks*: the total number of clicks on the short URL

**stats**: a table with simple statistics of the URLs  
*id*: the unique ID of the stat  
*url_id*: the ID of the URL  
*clickdate*: the date and time of this specific click  
*ip*: the IP address of the person clicking the URL  
*referer*: the referring web page

[You can download the SQL file here](https://github.com/dukeofharen/urlshortener.js/blob/master/urlshortener.sql).

Every Node.js application has one file which keeps everything together. I always call this file "app.js", but you can name it whatever you want. You can place this file in the root of the application.

Here is the "app.js" file:

    var express = require("express");
    var app = express();
    var router = require("./router");
    var con = require("./constants");
    
    app.use(express.bodyParser());
    app.use(express.urlencoded());
    app.use(express.json());
    
    app.listen(3500);
    console.log("Started listening at port 3500");
    router.route(app);


`require` is a way of telling Node.js to load a specific library, in this case "express". Now we have loaded Express, we can create a new instance of it, in the variable `app`. On the third and fourth line, 2 files called "router.js" and "constants.js" are "required". I've made these files myself and will explain that lateron. The section in the middle configures Express, so it can be used in combination with body parser (easy way to parse POST requests) and that JSON strings will be returned. The last section actually starts the server, listening at port `3500`. The code in the last line is calling a method which takes care of the routing (so d.co/add is actually creating a new short URL).

Before we can implement our logic, we have to create 2 files first (also in the root):

**constants.js**

I like to add a file with every Node.js application which contains static variables which usually don't change (e.g. database settings, number of URLs a user can create in 1 hour etc.). You can see this file below:

    exports.root_url = "http://localhost:3500/";
    exports.min_vanity_length = 4;
    exports.num_of_urls_per_hour = 50;
    
    exports.get_query = 'SELECT * FROM urls WHERE segment = {SEGMENT}';
    exports.add_query = 'INSERT INTO urls SET url = {URL}, segment = {SEGMENT}, ip = {IP}';
    exports.check_url_query = 'SELECT * FROM urls WHERE url = {URL}';
    exports.update_views_query = 'UPDATE urls SET num_of_clicks = {VIEWS} WHERE id = {ID}';
    exports.insert_view = 'INSERT INTO stats SET ip = {IP}, url_id = {URL_ID}, referer = {REFERER}';
    exports.check_ip_query = 'SELECT COUNT(id) as counted FROM urls WHERE datetime_added >= now() - INTERVAL 1 HOUR AND ip = {IP}';
    
    exports.host = 'localhost';
    exports.user = 'root';
    exports.password = 'password';
    exports.database = 'urlsh';


This file contains a few settings, under which a few predefined query's which we will use later in the application logic. One of the things you might find strange is the use of `exports`. Every variable, function, object or whatever you assign to exports, will be available after you `require` the file. So, when you do `var constants = require('./constants')`, you can, for example, call variable by using `constans.root_url`. This is what makes Node.js so flexible and modular.

Another thing you might notice is that the root URL of the application is defined in the variable `root_url`. This variable contains the URL of your actual short URL. This is actually done because a Node.js application is typically run side-by-side with an Apache or Nginx server, so the app can't run on port 80 (and the url d.co:3500 is very ugly in my opinion). It is more common to let your domain (let's say d.co) use a reverse proxy to point to your Node.js app. This is in my opinion too farfetched for this tutorial and I'm not going to explain it here (Google for "apache reverse proxy" or "nginx reverse proxy").

**router.js**

This file contains all the routes of the application. A route is for example "http://d.co/add". If the application detects this URL, it knows it should execute the action "add". I always declare these routes in the file "router.js". You'll see this file below:

    var logic = require('./logic');
    var route = function(app){
        app.get('/add', function(request, response){
            var url = request.param('url');
            var vanity = request.param('vanity');
            logic.addUrl(url, request, response, vanity);
        });
    
        app.get('/whatis', function(request, response){
            var url = request.param('url');
            logic.whatIs(url, request, response);
        });
    
        app.get('/:segment', function(request, response){
            logic.getUrl(request.params.segment, request, response);
        });
    }
    
    exports.route = route;


As you can see here, the file requires the file "logic.js" (which I'll explain in a moment). The file also defines a function "route" (which is called in "app.js") which contains all routes of the application.

The first route calls the logic which is needed to shorten a URL. The route function gets 2 objects through Express: a request and response object. These methods correspond to both the HTTP request and HTTP response. The method `request.param('url')` fetches the URL paramater `url`. It also fetches a URL variable called "vanity", which is used if a user wants to create a custom short URL.

The second route corresponds to the URL "http://d.co/whatis". If you pass a short URL (or just the segment at the end of the URL) with this URL, you get some basic statistics about this URL (e.g. the long URL, the number of clicks etc.).

The last route is the actual short URL. The `/:segment` places the variable at the position of `segment` in the variable `segment`, which can be called through `request.params.segment`.

Now we have both *router.js* and *constants.js*, it's time to create a new file: *logic.js*. But before we're going to begin, I've assembled a few (error) codes which will be returned by the application on some occations:

*   100: action executed successfully
*   400: couldn't add URL, most likely the vanity (or custom) url is already taken
*   401: the URL isn't reachable (e.g. a 404 or 500 is returned, or the URL doesn't exist at all)
*   402: the "URL" parameter isn't set
*   403: the vanity string contains invalid characters (valid characters are 0-9, a-z, A-Z. - and _)
*   404: the short link doesn't exist (this is returned when you call a short URL and it doesn't exist in the database)
*   405: the vanity string (custom short URL) can't be longer than 15 characters
*   406: the URL can't be longer than 1000 characters
*   407: the vanity string has to contain more characters
*   408: maximum number of URL's per hour exceeded (you can only create up to a certain number of short URLs per hour, this is defined in "constants.js")

Below you'll see the begin of the file **logic.js**:

    var mysql = require("mysql");
    var req = require("request");
    var cons = require("./constants");
    var crypto = require('crypto');
    var pool = mysql.createPool({
            host:cons.host,
            user:cons.user,
            password:cons.password,
            database:cons.database
        });


So what happens here?

*   The module mysql will be loaded for database interactions
*   The module request will be loaded for checking if a certain URL does exist
*   Our constants file will be loaded
*   The module crypto will be loaded, which will be used to generate a short URL
*   A MySQL connection pool is created with the connection data we've provided in "constants.js"

Let's continue with the rest of the code, I've placed comments at every function:

    //onSuccess: the method which should be executed if the hash has been generated successfully
    //onError: if there was an error, this function will be executed
    //retryCount: how many times the function should check if a certain hash already exists in the database
    //url: the url which should be shortened
    //request / response: the request and response objects
    //con: the MySQL connection
    //vanity: this should be a string which represents a custom URL (e.g. "url" corresponds to d.co/url)
    function generateHash(onSuccess, onError, retryCount, url, request, response, con, vanity) {
        var hash = "";
        if(vanity){
            hash = vanity;
            var reg = /[^A-Za-z0-9-_]/;
            //If the hash contains invalid characters or is equal to other methods ("add" or "whatis"), an error will be thrown
            if(reg.test(hash) || hash == "add" || hash == "whatis"){
                onError(response, request, con, 403);
                return;
            }
            if(hash.length > 15){
                onError(response, request, con, 405);
                return;
            }
            else if(cons.min_vanity_length > 0 && hash.length < cons.min_vanity_length){
                onError(response, request, con, 407);
                return;
            }
        }
        else{
            //This section creates a string for a short URL on basis of an SHA1 hash
            var shasum = crypto.createHash('sha1');
            shasum.update((new Date).getTime()+"");
            hash = shasum.digest('hex').substring(0, 8);
        }
        //This section query's (with a query defined in "constants.js") and looks if the short URL with the specific segment already exists
        //If the segment already exists, it will repeat the generateHash function until a segment is generated which does not exist in the database
        con.query(cons.get_query.replace("{SEGMENT}", con.escape(hash)), function(err, rows){
            if(err){
                console.log(err);
            }
            if (rows != undefined && rows.length == 0) {
                onSuccess(hash, url, request, response, con);
            } else {
                if (retryCount > 1 && !vanity) {
                    generateHash(onSuccess, onError, retryCount - 1, url, request, response, con);
                } else {
                    onError(response, request, con, 400);
                }
            }
        });
    }
    
    //The function that is executed when there's an error
    //response.send sends a message back to the client
    function hashError(response, request, con, code){
        response.send(urlResult(null, false, code));
    }
    
    //The function that is executed when the short URL has been created successfully.
    function handleHash(hash, url, request, response, con){
        con.query(cons.add_query.replace("{URL}", con.escape(url)).replace("{SEGMENT}", con.escape(hash)).replace("{IP}", con.escape(getIP(request))), function(err, rows){
            if(err){
                console.log(err);
            }
        });
        response.send(urlResult(hash, true, 100));
    }
    
    //This function returns the object that will be sent to the client
    function urlResult(hash, result, statusCode){
        return {
            url: hash != null ? cons.root_url+hash : null,
            result: result,
            statusCode: statusCode
        };
    }
    
    //This method looks handles a short URL and redirects to that URL if it exists
    //If the short URL exists, some statistics are saved to the database
    var getUrl = function(segment, request, response){
        pool.getConnection(function(err, con){
            con.query(cons.get_query.replace("{SEGMENT}", con.escape(segment)), function(err, rows){
                var result = rows;
                if(!err && rows.length > 0){
                    var referer = "";
                    if(request.headers.referer){
                        referer = request.headers.referer;
                    }
                    con.query(cons.insert_view.replace("{IP}", con.escape(getIP(request))).replace("{URL_ID}", con.escape(result[0].id)).replace("{REFERER}", con.escape(referer)), function(err, rows){
                        if(err){
                            console.log(err);
                        }
                        con.query(cons.update_views_query.replace("{VIEWS}", con.escape(result[0].num_of_clicks+1)).replace("{ID}", con.escape(result[0].id)), function(err, rows){
                            if(err){
                                console.log(err);
                            }
                        });
                    });
                    response.redirect(result[0].url);
                }
                else{
                    response.send(urlResult(null, false, 404));
                }
                if(err){
                    console.log(err);
                }
            });
            con.release();
        });
    };
    
    //This function adds attempts to add an URL to the database. If the URL returns a 404 or if there is another error, this method returns an error to the client, else an object with the newly shortened URL is sent back to the client.
    var addUrl = function(url, request, response, vanity){
        pool.getConnection(function(err, con){
            if(url){
                url = decodeURIComponent(url).toLowerCase();
                con.query(cons.check_ip_query.replace("{IP}", con.escape(getIP(request))), function(err, rows){
                    if(err){
                        console.log(err);
                    }
                    if(rows[0].counted != undefined && rows[0].counted < cons.num_of_urls_per_hour){
                        con.query(cons.check_url_query.replace("{URL}", con.escape(url)), function(err, rows){
                            if(err){
                                console.log(err);
                            }
                            if(url.indexOf("http://localhost") > -1 || url.indexOf("https://localhost") > -1){
                                response.send(urlResult(null, false, 401));
                                return;
                            }
                            if(url.length > 1000){
                                response.send(urlResult(null, false, 406));
                                return;
                            }
                            if(!err && rows.length > 0){
                                response.send(urlResult(rows[0].segment, true, 100));
                            }
                            else{
                                req(url, function(err, res, body){
                                    if(res != undefined && res.statusCode == 200){
                                        generateHash(handleHash, hashError, 50, url, request, response, con, vanity);
                                    }
                                    else{
                                        response.send(urlResult(null, false, 401));
                                    }
                                });
                            }
                        });
                    }
                    else{
                        response.send(urlResult(null, false, 408));
                    }
                });
            }
            else{
                response.send(urlResult(null, false, 402));
            }
            con.release();
        });
    };
    
    //This method looks up stats of a specific short URL and sends it to the client
    var whatIs = function(url, request, response){
        pool.getConnection(function(err, con){
            var hash = url;
            if(!hash) hash = "";
            hash = hash.replace(cons.root_url, "");
            con.query(cons.get_query.replace("{SEGMENT}", con.escape(hash)), function(err, rows){
                if(err || rows.length == 0){
                    response.send({result: false, url: null});
                }
                else{
                    response.send({result: true, url: rows[0].url, hash: hash, clicks: rows[0].num_of_clicks});
                }
            });
            con.release();
        });
    };
    
    //This function returns the correct IP address. Node.js apps normally run behind a proxy, so the remoteAddress will be equal to the proxy. A proxy sends a header "X-Forwarded-For", so if this header is set, this IP address will be used.
    function getIP(request){
        return request.header("x-forwarded-for") || request.connection.remoteAddress;
    }
    
    exports.getUrl = getUrl;
    exports.addUrl = addUrl;
    exports.whatIs = whatIs;


## Let's start the application

Using your command line, go to the folder where the URL shortenere resides and run command `node app.js`, you'll probably see something like `Started listening at port 3500`. Let's run some commands.

**Add URL**

If you call `http://localhost:3500/add?url=http%3A%2F%2Fgoogle.com`, you get something like this:

    {
      "url": "http://localhost:3500/d7d780db",
      "result": true,
      "statusCode": 100
    }


As you can see, the URL is URL encoded before it is being sent to the server. If you now browse to `http://localhost:3500/d7d780db`, you'll go to Google. Very easy and straightforward API call.

If you call `http://localhost:3500/add?url=http%3A%2F%2Fmicrosoft.com&vanity=microsoft`, you get a custom short URL:

    {
      "url": "http://localhost:3500/microsoft",
      "result": true,
      "statusCode": 100
    }


**Get statistics**

If you call `http://localhost:3500/whatis?url=http://localhost:3500/d7d780db` or `http://localhost:3500/whatis?url=d7d780db`, you'll see some simple statistics.

    {
      "result": true,
      "url": "http://google.com",
      "hash": "d7d780db",
      "clicks": 3
    }


## Other stuff

If you get an error and don't know how to fix it, please leave a comment. Maybe I can help you.

Also important to know: this script is meant for public use. If you want to use it only for yourself, you'll have to add some simple authentication (e.g. you send an extra authentication string which is checked everytime you call "add" or "whatis").

If you want to run this script on a server, you have to use a Node module like [forever](https://www.npmjs.com/package/forever). Normally, a Node.js app closes when you close the command line. This is no other on for example a Ubuntu server. If you use this module, you make sure there is still some sort of command line to which the Node.js application can talk.

Looking for a cool short domain? A lot of countries opened up their ccTLDs (country code top level domain) to the public. Codes like **.gr** (Greece), **.nu** (Niue), **.je** (Jersey), **.gs** (Georgia and South Sandwich Islands) and **.im** (Isle of Man) have a lot of cool short domains available, and aren't very expensive to order. I own **duc.gr** for example. For a complete overview of ccTLDs you can order, you can take a look here: <https://www.101domain.com/country_domain.htm>.

I've released the source code on GitHub under the MIT license, [you can download it here](https://github.com/dukeofharen/urlshortener.js).

## What to do now?

*   Create a nice GUI for people to shorten their URLs
*   Create graphs of the statistics
*   If you're going to use it for yourself, implement some type of authentication
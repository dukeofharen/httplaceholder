---
title: Node Webkit (NW.js) tutorial: creating a Markdown editor
date: 2015-02-21T21:36:19+02:00
description: Desktop apps are dead, right? Not really by judging the number of stars Node Webkit has gained on GitHub. While most apps are deployed in the cloud nowadays, a lot of work is still done on our little local computers. Tools like text editors, markdown editors, games etc. are still mostly used offline.
---

Version 0.1 (21 February 2015)

**[Download the source code here](https://github.com/dukeofharen/markdown-editor)**, or **[download the program (assembled for Windows)](https://mega.nz/#!0hMhSY6b!Hye-7H6vgPPp37OKUdewdZLdW-U4hqK8Ryw1RmThWqM)**

Desktop apps are dead, right? Not really by judging the number of stars Node Webkit has gained on GitHub. While most apps are deployed in the cloud nowadays, a lot of work is still done on our little local computers. Tools like text editors, markdown editors, games etc. are still mostly used offline.

I've said it before, but I really like the Markdown markup language. I thought it would be nice to create a very basic but functional Markdown editor. This tutorial teaches (some of) the basics of Node Webkit. I'll be using Windows, but the app can be deployed to other operating systems with minimal effort.

![](/static/img/md-editor/md-complete.jpg)

## Now, what is Node Webkit?

Node Webkit ([you can get it here](https://github.com/nwjs/nw.js)) is a stripped down of Chromium (the open source base of Google Chrome) with which you can create desktop applications (for Windows, Mac OSX and Linux) using nothing more than HTML, CSS and JavaScript. You can create great web apps, but when you want to do something like changing files on the OS, you just can't. This is where Node Webkit (like the name says; with the power of Node.js) comes to the rescue. You can use Node Modules like *fs* (to do actions on the file system of the OS) and *request* (for making AJAX calls to all kinds of web services without having to care about cross domain problems). Node Webkit has out of the box support for a few local databases (there are a lot of Node modules for connections with e.g. MySQL), and because it's base is Chromium, it has support for localStorage and sessionStorage (for very easy persisting).

## Getting started

To get started, you'll have to go here: <https://github.com/nwjs/nw.js>. Pick the correct download for your OS, I'll be using the version for Windows (x64). Node.js is bundled with Node Webkit, but it won't hurt you if you have Node.js installed on your system, so you can use the node package manager (npm) to install packages to your app.

After you've downloaded the correct zip file, extract the files to your favourite location. After that, create a `package.json` file. This file contains the configuration for your application, like the version, name, which modules should be installed, if the Webkit toolbar should be shown etc. Below, you'll see my file:

    {
      "name": "markdown_editor",
      "main": "./html/index.html",
      "window": {
        "toolbar": true,
        "show": true,
        "icon": "./img/icon.png"
      },
      "dependencies": {
        "marked": "^0.3.3"
      }
    }


*   `name`: this is the name for your application. The name should be without spaces.
*   `main`: this is the HTML file which will be loaded when the application starts.
*   `window`: this object contains information about the Webkit window.
    *   `toolbar`: whether the Webkit toolbar should be shown or not. This toolbar contains a refresh button, an address bar and a button leading to the Chromium development tools.
    *   `show`: whether the window should be opened when you open the app. In some instances, you don't want the window to open, e.g. when your app is only a system tray icon.
    *   `icon`: the icon which should be shown on the top of the window.
*   `dependencies`: the Node.js modules which should be installed.

The package.json file can contain many more properties. You should take a look here: <https://github.com/nwjs/nw.js/wiki/Manifest-format>.

I've created 4 new folders in the project root: `html`, `css`, `js` and `img` (I'm assuming you know what these mean ;)). I've placed a file named "icon.png" in the "img" folder (as mentioned in the package.json file). Below, you'll see a stub of 2 files (note: I've already placed jQuery in the js folder):

**html/index.html**

    <!DOCTYPE html>
    <html>
    <head>
        <title>Markdown Editor</title>
        <link rel="stylesheet" type="text/css" href="../css/screen.css" media="screen" />
        <script src="../js/jquery/jquery.min.js" type="text/javascript"></script>
        <script type="text/javascript" src="../js/main.js"></script>
    </head>
    <body>
        <textarea name="markdown" id="editor" class="md_editor"></textarea>
        <div class="md_result">
    
        </div>
    </body>
    </html>


**css/style.css**

    html, body{
        height:98%;
    }
    body{
        font-family:Arial;
    }
    a{
        text-decoration:none !important;
    }
    
    .md_editor{
        float:left;
        width:50%;
        height:100%;
        resize:none;
        padding:0;
        margin:0;
    }
    .md_result{
        float:left;
        width:49%;
        box-sizing:border-box;
        padding:10px 10px 13px 10px;
        margin:0;
        height:100%;
        overflow-y:scroll;
    }


You can already create the file `js/main.js`.

This is a very basic setup, and nothing much is happening right now. If you click "nw.exe" in the project root, the package.json file is read and it understood to open the "index.html" file. As of now, the interface contains a text editor at the left side and an empty div on the right side. As you can guess, the textarea on the left will contain our Markdown markup and HTML will be rendered in the right div.

## Adding the Markdown parser

The first thing we are going to do now is adding the Markdown parser to the project. Everytime you fill in a character in the textarea on the left, the Markdown of the text should be parsed and shown on the right. As you've seen before, we've filled in the dependency "marked" in `package.json`. The only thing we have to do is open the command line and go to the project directory (the directory that contains package.json). Here, you'll have to execute `npm install` (Node.js has to be installed for this to work). NPM will look at the dependencies and downloads them from the NPM repository. If you take a look in the project folder now, you'll see a folder named "node_modules". This folder contains "marked" (and any other modules that will be installed).

Now Marked is installed, we can call the module in the application. But we have to do something first. You have to add this script block to the `<head>` tag in `index.html`:

    <script type="text/javascript">
    global.window = window;
    global.$ = $;
    global.gui = require('nw.gui');
    init();
    </script>


This script block adds the `window` variable, `$` (jQuery) variable and the `gui` variable (the variable with which we can control the looks of the window, for example the menu bar of the webkit window) to the `global` variable. This is done because these variables are unavailable in a Node.js context otherwise. You can't, for example, call `window.document` from a Node.js file, but when you have assigned this variable to the `global` variable, you can call it like this: `global.window.document`. The `init()` function will be defined in a few moments.

In the file `js/main.js`, add an `init()` function. This will be empty for now. We'll have to create a file `js/editor.js`. This file will contain the logic for the Markdown editor. Begin by adding this piece of code to the file:

    exports.reload = function(){
        var marked = require("marked");
        marked.setOptions({
        renderer: new marked.Renderer(),
            gfm: true,
            tables: true,
            breaks: false,
            pedantic: false,
            sanitize: false,
            smartLists: true,
            smartypants: false
        });
        var resultDiv = global.$('.md_result');
        var textEditor = global.$('#editor');
        var text = textEditor.val();
        resultDiv.html(marked(text));
    };


In this file, we add the function `reload()` to the `exports` variable. This variable will be returned when the function `require()` is called. The `require()` function is called when you want to load a .js file or a Node.js module. The function requires the module "marked" and sets its options. One of the most important options is `sanitize`. If this is set to true, HTML attributes which you use between your Markdown will be encoded. This is not what you want, because Markdown can (and should, if necessary) be mixed with HTML. `resultDiv` is the div which should contain the rendered Markdown and `textEditor` is our textarea. `text` is the text from the textarea. The last line puts the rendered Markdown in the `resultDiv`.

Next, we have to edit `main.js`. Here you will see my `main.js` file as of now:

    function init(){
        global.$(global.window.document).ready(function(){
            var editor = require("./../js/editor.js");
            var textEditor = global.$('#editor');
            textEditor.bind('input propertychange', function() {
                editor.reload();
            });
        });
    }


When the window is done loading, the function within will be executed. The editor file will be "required" so we can call the method `reload()`. The path of the editor.js file is relative to the "index.html" file. Next, the editor (in var `textEditor`) will be called. We will bind an event called `input propertychange` to the textarea. This means that every time we change something in the textarea, the `reload()` function will be called so the new rendered HTML will be shown in the result div.

![](/static/img/md-editor/md1.jpg) *The Markdown editor as of now. The Markdown is successfully parsed.*

## Adding a menu

In this step, we will be adding a simple menu bar at the top of the Webkit window. This menu will contain a submenu with the buttons "New", "Open", "Save" and "Exit". Start by adding a file `menu.js`. Insert this code in menu.js:

    exports.initMenu = function(){
        var win = global.gui.Window.get();
        var menubar = new global.gui.Menu({ type: 'menubar' });
        var fileMenu = new global.gui.Menu();
        fileMenu.append(new global.gui.MenuItem({
            label: 'New',
            click: function() {
    
            }
        }));
        fileMenu.append(new global.gui.MenuItem({
            label: 'Open',
            click: function() {
    
            }
        }));
        fileMenu.append(new global.gui.MenuItem({
            label: 'Save',
            click: function() {
    
            }
        }));
        fileMenu.append(new global.gui.MenuItem({
            label: 'Exit',
            click: function() {
                global.gui.App.quit();
            }
        }));
    
        menubar.append(new global.gui.MenuItem({ label: 'File', submenu: fileMenu}));
        win.menu = menubar;
    };


As you can see here, the "win" object is being requested. This object is needed if you want to change certain things on the Webkit window (in this case add a menu bar). A menu bar is created and a submenu with different options is added to this bar. The only button which actually does something right now is "Exit". This closes the application (obviously). You have to add this piece of code to the `init()` method in `main.js`:

    var menu = require("./../js/menu.js");
    menu.initMenu();


I've added this piece of code to the top of `init()`. If you start the application now, you see a menu now and when you press "Exit", the application actually closes.

![](/static/img/md-editor/md2.jpg)

The next thing we have to do is add these 2 functions to `editor.js`:

    exports.loadText = function(text){
        var textEditor = global.$('#editor');
        textEditor.val(text);
        exports.reload();
    };
    
    exports.loadFile = function(file){
        var fs = require('fs');
        fs.readFile(file, 'utf8', function (err,data) {
            if (err) {
                return console.log(err);
            }
            exports.loadText(data);
        });
    };


The function `loadText()` replaces the current text in the textarea with a new one. When the text is replaced, `reload()` is called so the Markdown will be parsed and directly shown in the div on the right. The function `loadFile()` takes a path to a specific file as parameter and loads the contents of that file in the textarea.

We can implement the "file open" functionality now. Begin by adding this piece of HTML in the body tag of `index.html`:

    <input style="display:none;" id="openFileDialog" type="file" />


This is a hidden file dialog. We want this dialog to open when the "Open" button is clicked. Add the following function to `editor.js`:

    exports.chooseFile = function(name, callback) {
        var chooser = global.$(name);
        chooser.change(function(evt) {
            callback(global.$(this).val());
        });
    
        chooser.trigger('click');
    };


This function takes the name of the file dialog element (this will be "#openFileDialog" in our case) and the second parameter is the function which will be executed when the file has been selected. You have to "require" `editor.js` in the `initMenu()` function in `menu.js`:

    var editor = require("./../js/editor.js");


We can implement the "Open file" functionality in `menu.js` now:

    fileMenu.append(new global.gui.MenuItem({
        label: 'Open',
        click: function() {
            editor.chooseFile("#openFileDialog", function(filename){
                editor.loadFile(filename);
            });
        }
    }));


This piece of code calls the `chooseFile()` function of `editor.js`. If a file is selected, the file path will be passed to `loadFile()` and placed in the textarea. There's not much more to it.

![](/static/img/md-editor/md3.jpg)

We can also implement the "New" command now. When the command is called, the textarea will become empty and the result on the right will also be removed. With the functions we've already implemented, it takes only one line of code (within the "New" command button action):

    fileMenu.append(new global.gui.MenuItem({
        label: 'New',
        click: function() {
            editor.loadText("");
        }
    }));


An empty string is loaded within the editor and the Markdown result will be reloaded. Nothing more, nothing less.

The last functionality which we should implement in the menu is the "Save" button. When this button is pressed, a new file dialog will open and you can specify where you want to save the file. The first thing you'll have to do is to add a hidden save file dialog to `index.html` (just like you did with the file open dialog). You can place it beneath the open file dialog:

    <input style="display:none;" type="file"  id="saveFileDialog" nwsaveas="file.md" />


The `nwsaveas` sets the default name of the file. The following piece of code has to be added to `editor.js`:

    fileMenu.append(new global.gui.MenuItem({
        label: 'Save',
        click: function() {
            editor.chooseFile("#saveFileDialog", function(filename){
                var fs = require('fs');
                var textEditor = global.$('#editor');
                fs.writeFile(filename, textEditor.val(), function(err) {
                    if(err) {
                        console.log(err);
                    } else {
                        console.log("The file was saved!");
                    }
                }); 
            });
        }
    }));


In the "Save" button click, we can use the `chooseFile()` method. When the right file location and name have been filled in, the `fs` module will be loaded. The contents of the textarea will be retrieved and passed to the `writeFile()` method of `fs`.

That's it, we've built a simple but effective menu. Let's continue with some other stuff which will be cool to implement in the app.

## Other stuff

### File type associations

A cool functionality would be to associate the `.md` (Markdown) extension with our app. To do this, we will have to read the "arguments" passed to the application. We can do this by adding the following piece of code the `init()` function in `main.js` (in the `ready` event):

    if(global.gui.App.argv.length > 0){
        editor.loadFile(global.gui.App.argv[0]);
    }


If a file type is associated with our app and the file is opened, the contents of that file will be loaded to our textarea. It is that simple.

### Multiple tabs

A cool feature of for example Notepad++ is that you can select multiple multiple lines, press tab and all those lines will will move by one tab. If you press Shift + tab, those lines will move back one tab. I've found a nice JavaScript library which handles this for you: [Taboverride](https://github.com/wjbryant/taboverride). You should include this JavaScript file in `index.html`. When you've done this, you should add this piece of code in the `init()` function in `main.js` (in the `ready` event):

    tabOverride.set(global.window.document.getElementsByTagName('textarea'));


## Done

You've now created a very basic Markdown editor. Although this is a nice setup, it is far from complete. It needs a few new functions:

### Difference between "Save" and "Save as"

When you click "Save" now, it always opens a dialog where you can specify the location of the file. If you've already done "Save as", the app should memorize the location of that file and do a "Save" without a dialog.

### Save on closing

If you've edited a file and close the app (or open an existing or new file), you don't get a message if you want to save. You should keep a variable (for example named `changed`) which will jump from false to true if anything is typed in the textarea and from true to false if you've saved. Then, you can show a message if you close the current note.

### Shortcut keys

A nice new functionality would be to assign `Ctrl + S` to saving, `Ctrl + O` to open etc.

### Print (to PDF)

It would also be very nice if you can print. This can be done by exporting the current rendered Markup as PDF and printing it through the PDF reader. HTML to PDF can be done by using [PhantomJS](http://phantomjs.org/).

### Creating a toolbar

A nice MS Word like toolbar would be a cool feature for the Markdown editor. This toolbar can contain buttons like "Head", "Italic", "Bold" etc.

### Opening multiple instances

By default there can only be one active instance of the app. You should take a look at the `single-instance` property here: <https://github.com/nwjs/nw.js/wiki/Manifest-format>.

### Removing the debug bar

If you're developping, the debug bar is your best friend. If you're deploying your app, you would like it removed. You can remove the bar by setting the property `toolbar` in `package.json` to false.

### Redirecting external URLs

If you click on a link which leads to an external site, that site will be opened in the Webkit window. You should implement a function which will catch the URL clicks and open a new instance of your default browser instead.

### Custom CSS

You should give the users a possibility of editing the default CSS for the rendered Markdown in the div on the right.

### Packaging the editor

If you want the Markdown editor to be packaged (so a single .exe, .app or whatever), read this article: <https://github.com/nwjs/nw.js/wiki/How-to-package-and-distribute-your-apps>. This will explain everything you need to do to package and distribute your apps.

**[Download the source code here](https://github.com/dukeofharen/markdown-editor)**, or **[download the program (assembled for Windows)](https://mega.nz/#!0hMhSY6b!Hye-7H6vgPPp37OKUdewdZLdW-U4hqK8Ryw1RmThWqM)**
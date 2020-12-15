## Install HttPlaceholder on Linux

The only thing needed to use HttPlaceholder on your local development machine, is extracting the archive with the HttPlaceholder binaries (which can be found [here](https://github.com/dukeofharen/httplaceholder/releases/latest)). You need to put HttPlaceholder on your path variable yourself.

Alternatively, to install HttPlaceholder on Linux, run the following command in your terminal (make sure you're running as administrator):

```bash
curl -o- https://raw.githubusercontent.com/dukeofharen/httplaceholder/master/scripts/install-linux.sh | sudo bash
```

This is a self contained version of HttPlaceolder: no SDK has to be installed to run it.

If you would like to expose HttPlaceholder to the outside world, I would recommend to use Nginx or Apache as reverse proxy. To keep the service running even if you're not logged in through an SSH session, you can use something like systemd.

There are two Vagrant boxes (Windows and Ubuntu) that you can use to view how installation of HttPlaceholder is done. You can find them in this repository under the folder "vagrant". You need to have [Vagrant](https://www.vagrantup.com/) installed. After that, it's just a matter of going to the correct folder in your terminal and typing `vagrant up`. HttPlaceholder will then be installed under Windows or Ubuntu and can be reached by going to `http://localhost:8080` or `https://localhost:4430`.
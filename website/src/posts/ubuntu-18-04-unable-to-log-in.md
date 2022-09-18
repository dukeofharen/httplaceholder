---
title: Ubuntu 18.04: unable to log in after locking PC
date: 2019-09-08T23:11:00+02:00
description: Ubuntu 18.04: unable to log in after locking PC
---

Hi,

Ok, this was an interesting one. I want to write this down for myself and other people who might have this problem in the future.

Last thursday, while at work, I decided to get a cup of coffee, so naturally I locked my PC. After getting back and trying to log in, the spinner kept spinning and after a about 10-20 seconds I got the text "Authentication error". Crap.

After some digging around (on my mobile phone, because no working laptop), I came across this StackOverflow link: https://askubuntu.com/questions/1032624/ubuntu-18-04-authentication-error-on-loging-after-lock. Long story short, Ubuntu (and probably other Linux distros to) use the .Xauthority file. This file contains information about the currently logged in user. This file is being "watched" by the OS, by the Inotify process in particular. Whenever this file changes, the OS sees this and takes actions accordingly.

The file watch limit for Inotify is 128 by default, which is sufficient in most cases. I had, however, Visual Studio Code and IntelliJ running. Both of them use Inotify, so the limit of 128 had exceeded. That's why the login didn't work anymore. That's also the reason locking isn't a problem when I don't run these applications.

The fix is simple: increase the Inotify file watch value. Update (or add if it isn't there yet) the following value to the file `/etc/sysctl.d/99-sysctl.conf`: `fs.inotify.max_user_watches=1048576`

After I performed these changes and rebooted my machine, locking and logging in worked again fine (even with VSCode and IntelliJ open).
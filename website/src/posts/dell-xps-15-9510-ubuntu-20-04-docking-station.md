---
title: Dell XPS 15 9510 + Ubuntu 20.04 + Docking station
date: 2021-12-23T21:30:23+01:00
description: My experiences with installing Ubuntu 20.04 on a Dell XPS 15 9510 together with a Dell WD19S docking station.
---

Hi,

I have been working from my desktop PC for a few years now. It is a pretty powerful PC which still is strong enough to handle the workload I throw at it. I have, however, the need to occasionally take my PC with me, which of course means a laptop is the way to go. I bought a Dell XPS 15 9510, and since not much could be found online about installing Ubuntu on this machine + if it works with a docking station, I decided to write a bit about it.

# The laptop

I have been looking around a bit, and the [Dell XPS 15](https://www.dell.com/en-us/shop/dell-laptops/xps-15-laptop/spd/xps-15-9510-laptop) looks like a nice machine. There is one catch: it is pre-installed with Windows and most places mention it is a Windows-first machine. I'm a software developer and prefer to work under Ubuntu. After doing some digging around, I decided I wanted to take the risk, order a Dell XPS 15 and see how Ubuntu works on it.

![Dell XPS 15 9510](/static/img/xps/xps15.png)

To start, I have ordered an XPS 15 with the following specs:
- 11th Generation Intel Core i7-11800H (4.6 GHz, 8 cores)
- 1TB M.2 PCIe NVMe Solid State Drive
- NVIDIA(R) GeForce RTX(TM) 3050 Ti 4GB GDDR6
- Killer Wi-Fi 6 AX1650 (2x2) and Bluetooth 5.1

# Putting Ubuntu on the laptop

I received my laptop on the 21st of December. According to UPS, my package was still in China since the 8th, and I did not get any updates in the meantime, so I was a little surprised when the mailman knocked on my door to deliver the laptop. Oh well, it was a nice surprise, and I immediately started on checking the laptop out.

![Dell XPS 15 9510](/static/img/xps/package.jpg)
_I only made a picture of the packaging, I did not film a full unboxing video, sorry._

Since the XPS 15 is Windows-first, I wanted to make the laptop dual boot. If, for some reason, I need to do something that is not supported under Linux (e.g. update the BIOS or play a Windows-only game), I can still boot to Windows, but I will probably work under Ubuntu 99.9% of the time. When I booted into Windows and set up everything, I needed to make sure that I shrunk down the volume Windows is installed on, so I can install Ubuntu besides it. You can do this by going to "Disk Management" (look for it in the start menu), right-click the drive Windows is installed on and select "Shrink Volume...". Pick a value that is right for your Ubuntu partition and is still big enough for Windows to function. For more information, you can read this article: https://www.diskpart.com/windows-10/shrink-volume-windows-10-0528.html.

Next step was downloading [Ubuntu](https://ubuntu.com/#download) and writing it to a USB drive. Ubuntu has a great guide for writing the Ubuntu ISO to a USB drive, so I direct you to this guide: https://ubuntu.com/tutorials/install-ubuntu-desktop#1-overview.

![Dell XPS 15 9510](/static/img/xps/converter.jpg)
_The XPS 15 does not have any USB ports (only USB-C and Thunderbolt ports), so you need a converter to connect a USB device to your XPS. Luckily, one converter (HDMI/USB to USB-C) is packaged in the box._

When I started installing Ubuntu, I selected the option to install Ubuntu besides Windows. Ubuntu gave me the error "Turn off BitLocker" at a certain point. I did not have BitLocker enabled on the Windows installation and when I looked in the BitLocker settings of Windows, it also said BitLocker was not enabled. However, when looking in the Windows "Disk Management", it says the C drive was BitLocker encrypted. I really did not know what was going on here. When looking through the page [Ubuntu installation on computers running Windows and BitLocker turned on](https://discourse.ubuntu.com/t/ubuntu-installation-on-computers-running-windows-and-bitlocker-turned-on/15338), I read the reactions by "cabinetbob58" and "vvc". The conclusion was that you need to "enable BitLocker" and directly "disable BitLocker". I tried this, and now I see no mention any more of the C drive being encrypted, so that seems good. Installing Ubuntu now worked like a charm.

You can, of course, also decide that you do not need Windows at all. If this is the case, you can ignore most of the stuff above and use the whole hard drive for Ubuntu.

## Tweaking Ubuntu

Right off the bat, Ubuntu seemed to work just fine. I installed all my favorite software (the JetBrains IDEs, VirtualBox, Docker etc.) and confirmed everything was working OK. One of the things I did was enabling the "proprietary" (gross, I know) drivers of NVIDIA for the XPS. You can do this by opening the application "Software & Updates", selecting  "Additional Drivers" and selecting "Using NVIDIA driver metapackage from nvidia-driver-470 (proprietary, tested)". I occasionally run VirtualBox images with Windows and I know if I use the default X.Org X server drivers, they are pretty much unusable (I would like to hear from anyone if they have the same problems or if I seem to be doing something wrong, but I do not have these problems with the NVIDIA drivers).

![Dell XPS 15 9510](/static/img/xps/drivers.png)

# The docking station

I also ordered a docking station. I ordered the [Dell WD19S](https://www.dell.com/en-us/work/shop/dell-docking-station-wd19s-180w/apd/210-azbm/pc-accessories) because it seems to be a right fit when using any Dell laptop. I also have a Dell laptop for my work, so being able to switch laptops with only one USB-C cable is a really welcome improvement.

![Dell XPS 15 9510](/static/img/xps/docking-station.png)

I connected the following cables to my Docking station:

- LAN cable
- 2 screens: 1 on HDMI and 1 on display port
- My USB hub on the USB port

Sadly, it does not have a 3.5mm audio port, but the laptop does have this port and is only a small nuisance right now (so I have to switch two cables instead of one; I will survive this, no problem). I might look into audio over USB-C or USB, but there is no rush yet.

There was no information available anywhere on the internet (or that I could find anyway) if the docking station works together with Ubuntu. I connected it, and it works out of the box, on multiple screens, so that too was a very nice surprise.

I also tried to connect a third external monitor to the docking station. Although the screen was recognized, I could not send any signal to it before turning off another monitor.

# Conclusion

I have only used the laptop for two days, but already did some serious programming on it (.NET, MySQL, PHP, JavaScript etc.) and even some gaming ([Satisfactory](https://www.satisfactorygame.com/) and [OpenRCT2](https://openrct2.org/)) and I encountered no major problems at this point. I encountered the following issues though:
- When I play any piece of audio over the headphone jack, it all sounds fine. If I stop the playback, after a few seconds, I hear an annoying buzzing tone. I did not have this when I connected my audio set to my desktop PC or any other PC, so I suspect it might be an issue with the drivers.
- When I disconnect the docking station, and connect the docking station again, Ubuntu will recognize that the two extra screens are connected again, but the screens stay black. The only solution at that point is restarting the PC. This does not always seem to be an issue though.

![Dell XPS 15 9510](/static/img/xps/desk.jpg)
_This is my setup. Not the best quality, photo taken with my phone at 11 PM._

If you have any questions, feel free to drop me a message. Also send me a message if the information in this article is incomplete. Like I said, I only use the laptop for two days, so I will probably encounter more issues along the way. All in all, although I can imagine that Windows runs a bit more smoothly on the laptop, Ubuntu seems to be running just fine for what I like to do with it.

# Links

While researching, I came across the following links which gave me some valuable insights.

- https://old.reddit.com/r/linuxhardware/comments/pdv8j1/hows_linux_on_the_latest_2021_dell_xps_15_9510/
- https://old.reddit.com/r/DellXPS/comments/oke7ll/dell_xps_15_9510_linux_compatibility/
- https://github.com/kristinpaget/xps-15-9510-audio
- https://medium.com/@asad.manji/my-journey-installing-ubuntu-20-04-on-the-dell-xps-15-9500-2020-8ac8560373d1
- https://medium.com/@tylergwlum/my-journey-installing-ubuntu-18-04-on-the-dell-xps-15-7590-2019-756f738a6447
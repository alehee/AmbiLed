# AmbiLed

<p align="center">
  <img src="https://github.com/alehee/AmbiLed/blob/master/github_resources/test.gif">
</p>

## Description
AmibLed is a low budget Philips Ambilight copy for *WS2812B Leds + Arduino + PC* setup. System is based on PC Aplication which sends information to microcontroller that changes specific leds on strip. Led strip refreshes aroud 15 times per second (Arduino data sending is not optimized :/) but its enough to watch movies with cool colorful effect!

## Used technology
Technology I used for this project:
* C#
* C# WPF
* Arduino circuit
* C with FastLED library

## Table of Contents
* [1. Prerequisites](#prerequisites)
* [2. Software](#software)
  - [2.1. Download and Installation](#download-and-installation)
* [3. Hardware](#hardware)
  - [3.1. Arduino preparation](#arduino-preparation)
  - [3.2. LEDs preparation](#leds-preparation)
* [4. How To Use](#how-to-use)
  - [4.1. Arduino Script preparation](#arduino-script-preparation)
  - [4.2. Program preparation](#program-preparation)
  - [4.3. First use config](#first-use-config)
* [5. Known issues](#known-issues)

## Prerequisites
To run the whole system you need some things. Here is a list with required hardware to even start thinking about testing the software:
* **Mid-end PC** *(the program is Cyberpunk-like optimized!)*
* **Arduino Uno** *(or cheaper replacement)*
* **WS2812B Leds with adapter** [here's](https://www.temposlighting.com/guides/power-any-ws2812b-setup) a link with led strip powering instructions
* **USB-B cable** *(the printer cable)* to connect Arduino and PC
* **Some Arduino cables** to connect leds with arduino and adapter
* **Arduino resistor** about 1kΩ

## Software
If you are already stocked with required things you can prepare the program for your PC!

  ### Download and Installation
  There's two ways: you can download the master branch with code, check how it's working and compile whole application in *Visual Studio 2019*, or simply download it from link below and run it from *PcLedVisualization.exe*
  
  #### Version 1.1.0 *(in progress)*
  This version is still in development, you can download the master branch to check the code!
  
  #### Version 1.0 - Full Color Range Verion
  [Here's](https://drive.google.com/file/d/1cXYhwnzx4T2U43efEgOIxQ6MEpvLbJOK/view?usp=sharing) the download link for the newest version of the **Full Color Range Version** program
  
  In the *AmbiLed.zip* archive you can find also the arduino *AmbiLed.ino* code which we will discuss later! (It can also be found in master brach *ArduinoScript* folder)
  
  [Here's the latest project files](https://drive.google.com/file/d/1oSSAx8QGpx7L8nfB4fKkBkjZXaqRgypt/view?usp=sharing) for the **Full Color Range Version**
  
  #### What next?
  If the program runs with no problems you are good to go! Software preparation will be continued in *How To Use* section so [hop in](#how-to-use) if you want!

## Hardware
To run the whole system, as I wrote before, you need some **Arduino Uno with cable**, **WS2812B Leds with adapter**, **some Arduino cables** and a **resistor**!

  ### Arduino preparation

  **Before you start** preparing hardware check again if your adapter will hold the led strip power consumption. Again [here's](https://www.temposlighting.com/guides/power-any-ws2812b-setup) a powering guide for this type of led strips

  If we are good to go, here's the scheme of Arduino system you need to connect:

  <p align="center">
    <img src="https://github.com/alehee/AmbiLed/blob/master/github_resources/arduino_schema.png">
  </p>

  Just connect or solder like on scheme and it's done! Just like that!

  We will be setting up Arduino script in the next step so keep the circuit next to you!
  
  ### LEDs preparation
  If you already connected leds with Arduino you need to attach them to screen you want to use with AmbiLed. To do that try to fit led strip with the screen size. There's two main rules:
  1. We need to make a rectangle so there's two pairs of same leds count. For example - my left and right sides have both 33 leds and top/bottom sides have 51 leds.
  2. Led strip needs to start in the bottom-right corner of the screen
  
  After the measurement you can stick the leds to back of your screen starting from bottom-right corner and making sure that horizontal leds count matches and vertical leds count too. **Write down** the led numbers - we will need it for future steps
  
  For now we are done with hardware preps! Great job!

## How To Use
We still have some preparations to make, so here's the last three steps: **Arduino Script preparation**, **Program preparation** and **First use config**. We are close!

  ### Arduino Script preparation
  Then open the *AmbiLed.ino* file in [Arduino IDE](https://www.arduino.cc/en/software) (if you downloaded the master branch it is in *ArduinoScript* folder) and edit 4. line
  ```c
  #define NUM_LEDS    102 // replace 102 with your leds total amount
  ```
  
  ***Pro tip!***
   In 17. line the script sets brightness for leds, you can set it in 0-255 range, but 50 works best in my opinion. Edit it only if you want to experiment a little bit, and you have plenty of power under your jacket ;)
   ```c
   FastLED.setBrightness(50);
   ```
  
  After that you can connect your PC with Arduino and send script to the circuit by matching COM port and clicking arrow in top-left corner of the IDE. Here you can **write down** also which COM port you connected the Arduino. Program needs the number of COM port so if you will always connect circuit to the same USB port in your PC you will have one more problem off your head!
  
  If no errors occured we prepared the Arduino correctly and we are good to go!
  
  ### Program preparation
  Now you need to run the program and learn how current GUI works, here's the scheme
  
  <p align="center">
    <img src="https://github.com/alehee/AmbiLed/blob/master/github_resources/panel.png">
  </p>
  
  * ![Simple color](https://dummyimage.com/10x10/ffffff/ffffff) screen capture preview (it will freeze after you turn on the leds for optimalization)
  * ![Simple color](https://dummyimage.com/10x10/ffccff/ffccff) Arduino icon to remind that leds should start in bottom-right corner
  * ![Simple color](https://dummyimage.com/10x10/ac7339/ac7339) leds required direction
  * ![Simple color](https://dummyimage.com/10x10/99ff66/99ff66) error/program log
  * ![Simple color](https://dummyimage.com/10x10/ff0000/ff0000) captured screen coordinates, more about it later!
  * ![Simple color](https://dummyimage.com/10x10/ff8533/ff8533) horizontal and vertical led count you made
  * ![Simple color](https://dummyimage.com/10x10/ffff00/ffff00) buttons for led calibrate and screen capture
  * ![Simple color](https://dummyimage.com/10x10/00ff00/00ff00) little on-screen led test, led with number you wrote will turn white for a moment
  * ![Simple color](https://dummyimage.com/10x10/9900cc/9900cc) led log button for test only and **very important** COM port selector to connect with Arduino (like in [Arduino Script preparation](#arduino-script-preparation))
  * ![Simple color](https://dummyimage.com/10x10/1a75ff/1a75ff) button to start the connection with Arduino and leds, square turns green if it's on!
  
  ### First use config
  So you know the scheme now, ok then, let's start the last config and fire it on! There's few minor steps, but i promise we're almost there!
  
  1. Fill the ![Simple color](https://dummyimage.com/10x10/ff0000/ff0000) screen coords - if you have one screen connected you need to simply enter your resolution, but if there's more than one screen connected to PC you need to experiment a little bit with coords and *Calibrate* button. *Screen Start Coords* are top-left coords and *Screen End Coords* are the bottom-right coords. **Take your time** and fit it perfectly!
  2. Enter the ![Simple color](https://dummyimage.com/10x10/ff8533/ff8533) leds numbers so the program can fit specific color to specific led
  3. Click ![Simple color](https://dummyimage.com/10x10/ffff00/ffff00) *Calibrate* to get the leds on screen and *Capture* for screen capture, always in this order! If preview don't fit for your screen restart the program and do the 1. step again to get correct screen area captured
  4. Select the ![Simple color](https://dummyimage.com/10x10/9900cc/9900cc) COM port you connected with Arduino
  5. Make sure you are calibrated and the screen is beeing captured
  6. Now run the ![Simple color](https://dummyimage.com/10x10/1a75ff/1a75ff) *Send Leds* button and the leds should lit like on screen preview!
  
  <p align="center">
    <img src="https://github.com/alehee/AmbiLed/blob/master/github_resources/presentation.png">
  </p>
  
  Program will save inputed data after you click *Calibrate* so you don't need to fill textboxes every time!
  
  **Every next use** requires to perform again **3-6** steps!
  
  Now just find a colorful movie and enjoy!
  
## Known issues
At this moment I've still got some issues to fix or improve:
* Sometimes led strip turns off and I need to reset it manually, it's all abour **powering** the led strip or my low quality soldering problem :/
* Led colors are not specific, I'll try to fix the algorithm but you can also experiment with it by downloading the master branch!

## Changelog
What's new? Here's the list:

* **1.1.0** - in development
  * refreshed GUI

## Thank you!
Thank you for peeking at my project!

If you're interested check out my other stuff [here](https://github.com/alehee)

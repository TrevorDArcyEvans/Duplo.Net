# Duplo.Net
C/C++/Java Duplicate Source Code Block Finder

(http://duplo.sourceforge.net/)

A more up to date version is at:

https://github.com/TrevorDArcyEvans/duplo.git

This project is a line by line port of the above project to .NET Core 6

________________________________________________________________________________

Duplo (C/C++/Java Duplicate Source Code Block Finder)

Version 0.1.3 Readme File, May 2005
________________________________________________________________________________

## Contents
1. General Information
2. System Requirements
3. Download
4. Performance Measurements
5. Background
6. License
________________________________________________________________________________

### 1 General Information

Duplicated source code blocks can harm maintainability of software systems.
Duplo is a tool to find duplicated code blocks within multiple C/C++/Java 
source files.

#### 1.1 Sample Output

----------------------------------------------------------------

```
src\engine\geometry\simple\TorusGeometry.cpp(56)
src\engine\geometry\simple\SphereGeometry.cpp(54)
	pBuffer[currentIndex*size+3]=(i+1)/(float)subdsU;
	pBuffer[currentIndex*size+4]=j/(float)subdsV;
	currentIndex++;
	pPrimitiveBuffer->unlock();

src\engine\geometry\subds\SubDsGeometry.cpp(37)
src\engine\geometry\SkinnedMeshGeometry.cpp(45)
    pBuffer[i*size+0]=m_ct[0]->m_pColors[i*3];
    pBuffer[i*size+1]=m_ct[0]->m_pColors[i*3+1];
    pBuffer[i*size+2]=m_ct[0]->m_pColors[i*3+2];
```
--------------------------------------------------------------------------------

#### 1.2 Usage

```
NAME
        Duplo 0.3.0 - duplicate source code block finder


SYNOPSIS
        duplo [OPTIONS] [INTPUT_FILELIST_FILE] [OUTPUT_FILE]

DESCRIPTION
        Duplo is a tool to find duplicated code blocks in large
        C/C++/Java/C#/VB.Net software systems.

        -ml minimal block size in lines (default is 4)
        -mc minimal characters in line (default is 3)
            lines with less characters are ignored
        -ip ignore preprocessor directives
        -d  ignore file pairs with same name
        -xml output file in XML
        INTPUT_FILELIST_FILE file with list of source files
        OUTPUT_FILE output file

VERSION
       0.3.0

AUTHORS
       Christian M. Ammann (cammann@giants.ch)
       Trevor D'Arcy-Evans (tdarcyevans@hotmail.com)
```

#### 1.3 Feedback and Bug Reporting

Please send Feedback and Bug Reports to:

cammann@giants.ch

tdarcyevans@hotmail.com

#### 1.4 Source files text file generation

Generate a list of all files of a directory with:

Windows
```
dir /s /b /a-d *.cpp *.h > myFiles.lst
dir /s /b /a-d *.cs > myFiles.lst
```

UNIX
```
 find -name "*.cpp" > cppList.lst
 find -name "*.h" > includeList.lst
 cat cppList.lst includeList.lst > all.lst
```
________________________________________________________________________________

### 2 System Requirements

Duplo needs about two times the memory of the total source code size. So if
your Source code is about 120MB it requires 240MB.

________________________________________________________________________________

### 3 Download

Download Duplo [here.](https://github.com/TrevorDArcyEvans/Duplo.Net/releases)

________________________________________________________________________________

### 4 Performance Measurements

|System                   |Files  |Locs     |Time   |Hardware   |
|-------------------------|------:|--------:|------:|-----------|
|3D Game Engine           |275    |12211    |4sec   |3.4GHZ P4  |
|Quake2                   |266    |102740   |58sec  |3.4GHZ P4  |
|Computer Game            |5639   |754320   |34min  |3.4GHZ P4  |
|Linux Kernel 2.6.11.10   |17034  |4184356  |16h    |3.4GHZ P4  |

________________________________________________________________________________

### 5 Background

Duplo uses the techniques described in the paper 
[A Language Independent Approach for Detecting Duplicated Code](http://www.iam.unibe.ch/~scg/Archive/Papers/Duca99bCodeDuplication.pdf)
from Stéphane Ducasse, Matthias Rieger and Serge Demeyer to detect duplicated code blocks.

________________________________________________________________________________

### 6 Prerequisites
* .NET Core 6

________________________________________________________________________________

### 7 Getting started

```bash
$ git clone https://github.com/TrevorDArcyEvans/Duplo.Net.git
$ cd Duplo.Net
$ dotnet restore
$ dotnet build
```
________________________________________________________________________________

### 8 LICENSE

Duplo is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

Foobar is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with Foobar; if not, write to the Free Software
Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
________________________________________________________________________________
                                   2005, Christian M. Ammann (cammann@giants.ch)

# Introduction

Sample application for Smart TV to open HTTP links in an integrated browser of TV.

It listens PUT requests with a link, put it to the list of links (ListView from Xamarin.Forms) and opens selected link.

# Build

```
tizen build-cs
```

# Deploy

Connect to SmartTV:

```
C:\tizen-studio\tools\sdb.exe connect 192.168.0.106
```

Install to SmartTV:

```
tizen install -n .\link-opener\bin\Debug\tizen55\org.tizen.example.linkopener.tv-1.0.0.tpk
```

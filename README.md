# systemd-watchdog
systemd-watchdog

## Publish
`dotnet publish -o /opt/daemons/systemd-watchdog/`

## Running

Execute `dotnet run` on the command line to run as a console application.

## Running as a service
systemd-watchdog.service

copy the systemd-watchdog.service file to `~/.config/systemd/user/`.

`cp ../systemd-watchdog.service ~/.config/systemd/user/`

Run `systemctl start --user systemd-watchdog.service` to start.

### Output

This service writes "Hello Watchdog!" every 2 seconds. 
You can see this by running `systemctl status --user systemd-watchdog.service`

`systemctl is-active --user systemd-watchdog.service`

## Systemd

https://www.freedesktop.org/software/systemd/man/sd_notify.html
https://www.freedesktop.org/software/systemd/man/sd_watchdog_enabled.html
https://www.freedesktop.org/software/systemd/man/systemd.service.html

systemctl stop --user systemd-watchdog.service
sudo dotnet publish testservice/testservice.csproj -o /opt/daemons/systemd-watchdog/
cp systemd-watchdog.service ~/.config/systemd/user/
systemctl --user daemon-reload
systemctl start --user systemd-watchdog.service
systemctl status --user systemd-watchdog.service
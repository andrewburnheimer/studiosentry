StudioSentry
============

Detects when RemoteApp users are connected, and disconnects them, on
command.


License
-------

Commercial and private use are permitted. Distribution, modification,
and sublicensing are all forbidden. Copyright details can be found in
the file LICENSE.md.


Install
-------

Double-click the installer released as part of this project.

See below for notes regarding installation of the service as part of the
development workflow.


Startup
-------

StudioSentry must be run from within the context of the Windows Services
Control Manager.

1. **Start Menu**, **Control Panel**, **Administrative Tools**,
   **Services**

2. Find **StudioSentry** in the list of services

3. Click the **Start** button (green "play button" icon) along the
   top-bar of the Services window


Usage
-----

See web interface served on the API socket, typically 8080.


Contribute
----------

Please fork the GitHub project (https://github.com/andrewburnheimer/sdnbc),
make any changes, commit and push to GitHub, and submit a pull request.

After making any code changes, install the service application locally
using a command-line utility called <code>InstallUtil.exe</code>. 

1. **Start Menu**, **Visual Studio 2015**, **Visual Studio Tools**,
   Right-click **Developer Command Prompt for VS2015** and **Run as
   administrator**

2. Access the directory where the compiled executable file of
   StudioSentry is located.

3. <code>installutil.exe StudioSentry.exe</code>

Uninstall the service with:

* <code>installutil.exe /u StudioSentry.exe</code>

Debug the operation of the service by attaching the debugger to the
running service.

1. Transfer <code>msvsmon.exe</code> from <code>Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\Remote Debugger\</code>*<platform>*
   to the computer where the service will be tested on.

2. run <code>msvsmon.exe</code>

1. Build StudioSentry in the Debug configuration.

2. Install your service per instructions above.

3. Start the service per instructions above.

4. Start Visual Studio with administrative credentials so you can attach
   to system processes.

5. On the menu bar, choose **Attach to Process** from the Debug or Tools
   menu.

6. In the Available Processes section, choose the process for your
   service, and then choose Attach. Choose the appropriate options, and
   then choose OK to close the dialog box.

You may also use the "Just-in-time" debugger with the service:

1. After the service has been started, open **Task Manager**

2. Find the name of the service in the **Processes** tab, right-click
   it, select Debug

3. Click **Attach debugger**, select the debugger you would like to use,
   and click the **Yes** button.


Contact
-------

This project was initiated by Andrew Burnheimer.

* Email:
  * andrew.burnheimer@nbcuni.com
* Twitter:
  * @aburnheimer
* Github:
  * https://github.com/andrewburnheimer/

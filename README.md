Installation
How to install plugins:
Put the plugin's .cs file into the ./plugins/ folder. If it's not there, make a new folder and put it in there.
Either in-game or via the server console, type /pcompile [plugin name]. It should say Plugin compiled successfully., if it doesn't, make an issue here.
Now type /pload [plugin name]. It should say Plugin loaded successfully.
And you're done, enjoy your plugin.
How to install commands:
Put the command's .cs file into the ./extra/commands/source/ folder. If it's not there, make a new folder and put it in there.
Either in-game or via the server console, type /compile [command name]. It should say Command compiled successfully., if it doesn't, make an issue here.
Now type /cmdload [command name]. It should say Command loaded successfully.
And you're done, enjoy your command.
NOTE: If you want your commands to load upon server start, you should add the command name into the ./text/cmdautoload.txt file.

For example, if your command file is named CmdHelloWorld.cs, you would put HelloWorld on a new line.

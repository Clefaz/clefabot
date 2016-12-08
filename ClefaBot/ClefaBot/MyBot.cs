using Discord;
using Discord.Commands;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClefaBot
{
    class MyBot
    {
        //VARIABLES
        DiscordClient discord;
        CommandService Commands;

        Random rand;

        string[] dankMemes;

        //MAIN
        public MyBot()
        {
            rand = new Random(); //Initialisation du random

            dankMemes = new string[]    //Dictionaire des memes
            {
                "DankMemes/(1).jpg",
                "DankMemes/(1).png",
                "DankMemes/(2).jpg",
                "DankMemes/(2).png",
                "DankMemes/(3).jpg",
                "DankMemes/(3).png",
                "DankMemes/(4).jpg",
                "DankMemes/(4).png",
            };

            discord = new DiscordClient(x =>    //Definition du client
            {
                x.LogLevel = LogSeverity.Info;
                x.LogHandler = Log;
            });

            discord.UsingCommands(x =>          //Definition des commandes
            {
                x.PrefixChar = '!';
                x.AllowMentionPrefix = true;
            });

            Commands = discord.GetService<CommandService>();

            //Commandes
            FunCommands();
            CoreCommands();
            PurgeCommand();
            DelallCommand();
            //FinCommandes

            discord.ExecuteAndWait(async () =>              //Connexion
            {
                await discord.Connect("MjIzODAwNzY0NjY4MDUxNDU2.CyHxrQ.olu66-hEbTgZuAdwiang6EKSAuw", TokenType.Bot);
            });
        }
        
        private void FunCommands()
        {
            //  !meme
            Commands.CreateCommand("meme")
                .Do(async (e) =>
                {
                    string selectedMeme = dankMemes[rand.Next(dankMemes.Length)];
                    await e.Channel.SendFile(selectedMeme);
                });

            //  !love
            Commands.CreateCommand("I love u")
                .Alias(new string[] { "je t'aime", "love", "i love you", "on baise" })
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("Je t'aime aussi" + e.User.Mention);
                });
        }

        private void CoreCommands()
        {
            //  !test
            Commands.CreateCommand("test")                 //Commande !Hello
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("yup ! I'm here !");     //Repondre Hi!
                });

            //  !help
            Commands.CreateCommand("help")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("`**HELP :**\n-1. truc\n-2. truc`");
                });
        }

        private void PurgeCommand()
        {
            //  !purge
            Commands.CreateCommand("purge")
                .Do(async (e) =>
                {
                    //verification des permissions
                    if (e.User.ServerPermissions.ManageMessages)
                    {
                        if (e.Channel.Name == "presentation")
                        {
                            //suppression des messages non autorisés
                            Message[] messagesReceived = await e.Channel.DownloadMessages(100);
                            for (int i = 0; i < messagesReceived.Length; i++)
                            {
                                if (!messagesReceived.ElementAt(i).Text.Contains("!pres"))
                                    await messagesReceived[i].Delete();
                            }
                        }
                        else if (e.Channel.Name == "aide_et_requetes")
                        {
                            //suppression des messages non autorisés
                            Message[] messagesReceived = await e.Channel.DownloadMessages(100);
                            for (int i = 0; i < messagesReceived.Length; i++)
                            {
                                if (!(messagesReceived.ElementAt(i).Text.Contains("!aide") || messagesReceived.ElementAt(i).Text.Contains("!requete")))
                                    await messagesReceived[i].Delete();
                            }
                        }
                        else
                            await e.Channel.SendMessage("Désolé, " + e.User.Mention + ", mais tu n'es pas dans le channel #presentation ou #aide_et_requete");
                    }
                    else
                        await e.Channel.SendMessage("Désolé, " + e.User.Mention + ", mais tu n'a pas la permission d'utiliser cette commande");
                });
        }

        private void DelallCommand()
        {  
            //  !delall
            Commands.CreateCommand("delall")
              .Alias(new string[] { "delall force" })
              .Do(async (e) =>
              {
                  //verification des permissions
                  if (e.User.ServerPermissions.ManageMessages)
                  {
                      if (e.Message.Text.Contains("force"))
                      {
                          Message[] messagesReceived = await e.Channel.DownloadMessages(100);   //les 100 derniers messages
                          await e.Channel.DeleteMessages(messagesReceived);
                      }
                      else
                      {
                          await e.Channel.SendMessage("Cette commande effacera les 100 derniers messages de ce channel, etes vous sur de vouloir les supprimer ?\n`!delall force`");
                      }
                  }
              });
        }

        private void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}

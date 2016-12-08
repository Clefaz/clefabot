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
                    await e.User.SendMessage("**HELP:**\n`AIDE ICI\nLOL`");
                    await e.Message.Delete();
                });

            //  !idea
            Commands.CreateCommand("idea")
                .Alias(new string[] { "idee" })
                .Parameter("idea", ParameterType.Multiple)
                .Do(async (e) =>
                {
                    string renvoi = e.Message.Text;

                    renvoi = renvoi.Replace("!idee", "");
                    renvoi = renvoi.Replace("!idea", "");

                    await e.Channel.SendMessage("idée de " + e.User.Mention + " : " + renvoi);
                    await e.Message.Delete();

                    Message[] messagesReceived = await e.Channel.DownloadMessages(100);
                    for (int i = 0; i < messagesReceived.Length; i++)
                    {
                        if (!messagesReceived.ElementAt(i).User.IsBot)
                            await messagesReceived[i].Delete();
                    }
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
                        {
                            await e.Channel.SendMessage("Désolé, " + e.User.Mention + ", mais tu n'es pas dans le channel #presentation ou #aide_et_requete");
                            await e.Message.Delete();
                        }
                    }
                    else
                    {
                        await e.Channel.SendMessage("Désolé, " + e.User.Mention + ", mais tu n'a pas la permission d'utiliser cette commande");
                        await e.Message.Delete();
                    }
                });
        }

        private void DelallCommand()
        {
            //  !delall
            Commands.CreateCommand("delall")
              .Parameter("force", ParameterType.Optional)
              .Do(async (e) =>
              {
                  //verification des permissions
                  if (e.User.ServerPermissions.ManageMessages)
                  {
                      if (e.GetArg("force") == "force")
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

            //  !del
            Commands.CreateCommand("del")
              .Parameter("nombre", ParameterType.Multiple)
              .Do(async (e) =>
              {
                  //verification des permissions
                  if (e.User.ServerPermissions.ManageMessages)
                  {
                      Role[] Roles = new Role[e.Server.RoleCount];
                      for (int i = 0; i < e.Server.RoleCount; i++)
                      {
                          Roles[i] = e.Server.Roles.ElementAt(i);
                          Console.WriteLine(Roles[i].Name);
                      }



                      //Debut de la fonction
                      if (e.GetArg("nombre") == "all")
                      {

                      }
                      else
                      {
                          int nombre;
                          if (Int32.TryParse(e.GetArg("nombre"), out nombre))
                          {
                              Message[] messagesReceived = await e.Channel.DownloadMessages(nombre + 1);   //les 2 derniers messages
                              await e.Channel.DeleteMessages(messagesReceived);
                          }
                          else
                              Console.WriteLine("String could not be parsed.");
                      }
                      //Fin de la fonction
                  }
                  else
                      await e.Channel.SendMessage("Cette commande effacera les x derniers messages de ce channel, etes vous sur de vouloir les supprimer ?\n`!del force`");
              });
        }

        private void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}

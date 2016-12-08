﻿using Discord;
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
            DelCommand();
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
                    await e.Channel.SendMessage(e.User.Mention);
                    await e.Channel.SendFile(selectedMeme);
                    await e.Message.Delete();
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
            Commands.CreateCommand("test")                 //Commande
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("It's working, " + e.User.Mention);     //Reponse
                    await e.Message.Delete();
                });

            //  !help
            Commands.CreateCommand("help")
                .Do(async (e) =>
                {
                    await e.User.SendMessage("**HELP:**\n\n`!del <number> [user(s)] [role(s)]\n->supprime < number > messages envoyés par les[user(s)] ou[role(s)]\n\n!del all < force >\n->supprime les 100 derniers messages\n\n!purge\n->supprime les 100 derniers messages ne contenant pas:\n\t[!pres] pour le channel presentation\n\t[!aide] / [!requete] pour le channel aide_et_requete\n\n!test\n->reponds pour indiquer que le bot fonctionne\n\n!help\n->affiche cette page\n\n!meme\n->poste un meme random\n\n!love / !I love u / !I love you / !on baise / !je t'aime\n->reponds un message plein d'amour\n`\n\n*copyright by clefaz: http://clefaz.com*");
                    await e.Message.Delete();
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

        private void DelCommand()
        {
            //  !del
            Commands.CreateCommand("del")
              .Parameter("nombre", ParameterType.Multiple)
              .Do(async (e) =>
              {
                  //verification des permissions
                  if (e.User.ServerPermissions.ManageMessages)
                  {
                      if (e.GetArg("nombre") == "?")
                          await e.Channel.SendMessage("Commande:\n`!del <number> [user(s)] [role(s)]\n!del all <force>`");
                      else { 
                      int nombre = 0;
                      int v = 0;

                          if (Int32.TryParse(e.GetArg("nombre"), out nombre))
                          {
                              Message[] messagesReceived = await e.Channel.DownloadMessages(100);   //charge les 100 derniers messages
                              await e.Message.Delete();                                             //supprime le message de commande

                              //
                              foreach (User user in e.Message.MentionedUsers)                       //supprime les messages pour les utilisateurs mentionnés
                              {
                                  for (int i = 1; i < 100; i++)                                     //On parcours la liste
                                  {
                                      if (messagesReceived[i].User.Id == user.Id)                   //Si le message est envoyé par l'utilisateur mentionné
                                      {
                                          v++;                                                      //On supprime son message
                                          Console.WriteLine("deleted " + v + " messages");          //
                                      }
                                      if (v == nombre)                                              //apres avoir supprimé les x derniers messages de l'utilisateur mentionné
                                      {
                                          v = 0;                                                    //reset v
                                          break;                                                    //on arrete le for
                                      }
                                  }
                              }

                              //
                              foreach (Role role in e.Message.MentionedRoles)                       //supprime les messages pour les utilisateurs mentionnés
                              {
                                  for (int i = 1; i < 100; i++)                                     //On parcours la liste
                                  {
                                      if (messagesReceived[i].User.HasRole(role))                   //Si le message est envoyé par l'utilisateur mentionné
                                      {
                                          v++;                                                      //On supprime son message
                                          Console.WriteLine("deleted " + v + " messages");          //
                                      }
                                      if (v == nombre)                                              //apres avoir supprimé les x derniers messages de l'utilisateur mentionné
                                      {
                                          v = 0;                                                    //reset v
                                          break;                                                    //on arrete le for
                                      }
                                  }
                              }

                              //
                              if (e.Message.Text.Contains("link"))
                              {
                                  for (int i = 1; i < 100; i++)                                     //On parcours la liste
                                  {
                                      if (messagesReceived[i].Text.Contains("http") || messagesReceived[i].Text.Contains("https"))                   //Si le message contient un lien
                                      {
                                          v++;                                                      //On supprime le message
                                          Console.WriteLine("deleted " + v + " messages");          //
                                      }
                                      if (v == nombre)                                              //apres avoir supprimé les x derniers messages avec un lien
                                      {
                                          v = 0;                                                    //reset v
                                          break;                                                    //on arrete le for
                                      }
                                  }
                              }
                          }
                          else
                          {
                              Console.WriteLine("String could not be parsed.");
                              if (e.GetArg("nombre") == "all")
                              {
                                  if (e.Message.Text.Contains("force"))
                                  {
                                      Message[] messagesReceived = await e.Channel.DownloadMessages(100);   //les 100 derniers messages
                                      await e.Channel.DeleteMessages(messagesReceived);
                                  }
                                  else
                                  {
                                      await e.Channel.SendMessage(e.User.Mention + "Cette commande effacera les 100 derniers messages de ce channel, etes vous sur de vouloir les supprimer ?\n`!del all force`");
                                  }
                              }
                              else
                                  await e.Channel.SendMessage("Commande invalide, " + e.User.Mention + ", tapez !help pour obtenir une liste de commandes.");
                          }
                      }
                  }
                  else {
                      await e.Channel.SendMessage("Désolé, " + e.User.Mention + ", mais tu n'a pas la permission d'utiliser cette commande");
                  }
                  await e.Message.Delete();
              });
        }

        private void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}

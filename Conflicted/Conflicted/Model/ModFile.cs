using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Conflicted.Model
{
    class ModFile
    {
        public string Path { get; }
        public string ID { get; }
        public string Name { get; }
        public string NameWithExtension { get; }
        public string Extension { get; }
        public string Directory { get; }
        public string Text { get; }

        public Mod Mod { get; }

        private IEnumerable<ModElement> elements;
        public IEnumerable<ModElement> Elements => elements ?? (elements = Interpret().ToList().AsReadOnly());

        private IEnumerable<ModFile> conflicts;
        public IEnumerable<ModFile> Conflicts
        {
            get
            {
                return conflicts ?? (conflicts = Mod.Modlist.FileConflicts.Where(group => group.Key == ID)
                    .SelectMany(group => group)
                    .Where(file => file != this)
                    .ToList()
                    .AsReadOnly());
            }
        }

        public ModFile(Mod mod, string path)
        {
            Mod = mod ?? throw new ArgumentNullException(nameof(mod));
            Path = path ?? throw new ArgumentNullException(nameof(path));

            ID = path.Remove(0, mod.DirPath.Length + 1);
            Name = System.IO.Path.GetFileNameWithoutExtension(path);
            NameWithExtension = System.IO.Path.GetFileName(path);
            Extension = System.IO.Path.GetExtension(path);
            Directory = new DirectoryInfo(path).Parent.Name;
            Text = Extension == ".txt" ? File.ReadAllText(path) : null;
        }

        public override string ToString()
        {
            return Name;
        }

        private IEnumerable<ModElement> Interpret()
        {
            if (Extension != ".txt")
            {
                return Enumerable.Empty<ModElement>();
            }
            
            switch (Directory)
            {
                case "anomalies":
                case "archaeological_site_types":
                case "armies":
                case "artifact_actions":
                case "ascension_perks":
                case "asteroid_belts":
                case "attitudes":
                case "bombardment_stances":
                case "buildings":
                case "bypass":
                case "casus_belli":
                case "colony_automation":
                case "colony_automation_exceptions":
                case "colony_types":
                case "colors":
                case "component_slot_templates":
                case "country_customization":
                case "country_types":
                case "decisions":
                case "deposit_categories":
                case "deposits":
                case "diplo_phrases":
                case "diplomatic_actions":
                case "districts":
                case "economic_categories":
                case "economic_plans":
                case "edicts":
                case "ethics":
                case "event_chains":
                case "fallen_empires":
                case "federation_law_categories":
                case "federation_laws":
                case "federation_perks":
                case "federation_types":
                case "galactic_focuses":
                case "game_rules":
                case "governments":
                case "authorities":
                case "civics":
                case "graphical_culture":
                case "leader_classes":
                case "mandates":
                case "map_modes":
                case "megastructures":
                case "name_lists":
                case "notification_modifiers":
                case "observation_station_missions":
                case "opinion_modifiers":
                case "personalities":
                case "planet_classes":
                case "planet_modifiers":
                case "policies":
                case "pop_categories":
                case "pop_faction_types":
                case "pop_jobs":
                case "precursor_civilizations":
                case "relics":
                case "resolution_categories":
                case "resolutions":
                case "scripted_effects":
                case "scripted_triggers":
                case "sector_focuses":
                case "sector_types":
                case "ship_sizes":
                case "solar_system_initializers":
                case "species_archetypes":
                case "species_classes":
                case "species_rights":
                case "star_classes":
                case "starbase_buildings":
                case "starbase_levels":
                case "starbase_modules":
                case "starbase_types":
                case "static_modifiers":
                case "strategic_resources":
                case "subjects":
                case "system_types":
                case "trade_conversions":
                case "tradition_categories":
                case "traditions":
                case "traits":
                case "war_goals":
                    return InterpretNamedBlock(0, Directory);

                case "portraits":
                    return InterpretNamedBlock(1, Directory);

                case "defines":
                    return InterpretKeyValueBlock(0, Directory);

                case "events":
                    return InterpretKeyedBlock(0, Directory, "id");

                case "component_sets":
                case "component_templates":
                case "random_names":
                case "section_templates":
                case "special_projects":
                    return InterpretKeyedBlock(0, Directory, "key");
                
                case "global_ship_designs":
                case "scripted_loc":
                case "ship_behaviors":
                    return InterpretKeyedBlock(0, Directory, "name");

                default:
                    return InterpretIgnore();
            }
        }

        private IEnumerable<ModElement> InterpretIgnore()
        {
            return Enumerable.Empty<ModElement>();
        }

        private IEnumerable<ModElement> InterpretKeyValueBlock(int blockLevel, string kind)
        {
            string text = File.ReadAllText(Path);

            Mode mode = Mode.Code;
            int level = 0, wordStart = -1, wordEnd = -1, stringStart = -1, stringEnd = -1, elementStart = -1, elementNameStart = -1, elementNameEnd = -1;
            List<string> lastDefines = new List<string>();

            for (int i = 0; i < text.Length; i++)
            {
                switch (mode)
                {
                    case Mode.Code:
                        if (text[i] == '#')
                        {
                            mode = Mode.Comment;
                        }
                        else if (text[i] == '"')
                        {
                            mode = Mode.String;
                            stringStart = i;
                        }
                        else if (text[i] == '=')
                        {
                            if (level == blockLevel && wordStart >= 0 && wordStart < wordEnd && wordStart > stringStart)
                            {
                                elementNameStart = wordStart;
                                elementNameEnd = wordEnd;
                                elementStart = wordStart;
                            }
                            if (level == blockLevel && stringStart >= 0 && stringStart < stringEnd && stringStart > wordStart)
                            {
                                elementNameStart = stringStart;
                                elementNameEnd = stringEnd;
                                elementStart = stringStart;
                            }
                            if (level == blockLevel + 1 && wordStart >= 0 && wordStart < wordEnd && wordStart > stringStart)
                            {
                                lastDefines.Add(text.Substring(wordStart, wordEnd - wordStart));
                            }
                            if (level == blockLevel + 1 && stringStart >= 0 && stringStart < stringEnd && stringStart > wordStart)
                            {
                                lastDefines.Add(text.Substring(stringStart, stringEnd - stringStart));
                            }
                        }
                        else if (text[i] == '{')
                        {
                            level++;
                        }
                        else if (text[i] == '}')
                        {
                            level--;

                            if (level == blockLevel && elementNameStart >= 0 && elementNameStart < elementNameEnd)
                            {
                                int elementEnd = i + 1;
                                foreach (var define in lastDefines)
                                {
                                    yield return new ModElement(this, $"{kind}::{text.Substring(elementNameStart, elementNameEnd - elementNameStart)}::{define}", text.Substring(elementStart, elementEnd - elementStart));
                                }
                                lastDefines = new List<string>();
                            }
                        }
                        else if (!char.IsWhiteSpace(text[i]))
                        {
                            mode = Mode.Word;
                            wordStart = i;
                        }
                        break;

                    case Mode.Word:
                        if (char.IsWhiteSpace(text[i]))
                        {
                            mode = Mode.Code;
                            wordEnd = i;
                        }
                        break;

                    case Mode.String:
                        if (text[i] == '"' && text[i - 1] != '\\')
                        {
                            mode = Mode.Code;
                            stringEnd = i + 1;
                        }
                        break;

                    case Mode.Comment:
                        if (text[i] == '\n')
                        {
                            mode = Mode.Code;
                        }
                        break;
                }
            }
        }

        private IEnumerable<ModElement> InterpretKeyedBlock(int blockLevel, string kind, string key)
        {
            string text = File.ReadAllText(Path);

            Mode mode = Mode.Code;
            int level = 0, wordStart = -1, wordEnd = -1, stringStart = -1, stringEnd = -1, elementStart = -1, elementNameStart = -1, elementNameEnd = -1;
            bool nextWordIsID = false;

            for (int i = 0; i < text.Length; i++)
            {
                switch (mode)
                {
                    case Mode.Code:
                        if (text[i] == '#')
                        {
                            mode = Mode.Comment;
                        }
                        else if (text[i] == '"')
                        {
                            mode = Mode.String;
                            stringStart = i;
                        }
                        else if (text[i] == '=')
                        {
                            if (level == blockLevel && wordStart >= 0 && wordStart < wordEnd && wordStart > stringStart)
                            {
                                elementStart = wordStart;
                            }
                            if (level == blockLevel && stringStart >= 0 && stringStart < stringEnd && stringStart > wordStart)
                            {
                                elementStart = stringStart;
                            }
                            if (level == blockLevel + 1 && wordStart >= 0 && wordStart < wordEnd && text.Substring(wordStart, wordEnd - wordStart) == key ||
                                level == blockLevel + 1 && stringStart >= 0 && stringStart < stringEnd && text.Substring(stringStart, stringEnd - stringStart) == key)
                            {
                                nextWordIsID = true;
                            }
                        }
                        else if (text[i] == '{')
                        {
                            level++;
                        }
                        else if (text[i] == '}')
                        {
                            level--;

                            if (level == blockLevel && elementStart >= 0 && elementStart < elementNameEnd)
                            {
                                int elementEnd = i + 1;
                                yield return new ModElement(this, $"{kind}::{text.Substring(elementNameStart, elementNameEnd - elementNameStart)}", text.Substring(elementStart, elementEnd - elementStart));
                            }
                        }
                        else if (!char.IsWhiteSpace(text[i]))
                        {
                            mode = Mode.Word;
                            wordStart = i;
                        }
                        break;

                    case Mode.Word:
                        if (char.IsWhiteSpace(text[i]))
                        {
                            mode = Mode.Code;
                            wordEnd = i;

                            if (nextWordIsID)
                            {
                                nextWordIsID = false;
                                elementNameStart = wordStart;
                                elementNameEnd = wordEnd;
                            }
                        }
                        break;

                    case Mode.String:
                        if (text[i] == '"' && text[i - 1] != '\\')
                        {
                            mode = Mode.Code;
                            stringEnd = i + 1;

                            if (nextWordIsID)
                            {
                                nextWordIsID = false;
                                elementNameStart = stringStart;
                                elementNameEnd = stringEnd;
                            }
                        }
                        break;

                    case Mode.Comment:
                        if (text[i] == '\n')
                        {
                            mode = Mode.Code;
                        }
                        break;
                }
            }
        }

        private IEnumerable<ModElement> InterpretNamedBlock(int blockLevel, string kind)
        {
            string text = File.ReadAllText(Path);

            Mode mode = Mode.Code;
            int level = 0, wordStart = -1, wordEnd = -1, stringStart = -1, stringEnd = -1, elementStart = -1, elementNameStart = -1, elementNameEnd = -1;

            for (int i = 0; i < text.Length; i++)
            {
                switch (mode)
                {
                    case Mode.Code:
                        if (text[i] == '#')
                        {
                            mode = Mode.Comment;
                        }
                        else if (text[i] == '"')
                        {
                            mode = Mode.String;
                            stringStart = i;
                        }
                        else if (text[i] == '=')
                        {
                            if (level == blockLevel && wordStart >= 0 && wordStart < wordEnd && wordStart > stringStart)
                            {
                                elementNameStart = wordStart;
                                elementNameEnd = wordEnd;
                                elementStart = wordStart;
                            }
                            if (level == blockLevel && stringStart >= 0 && stringStart < stringEnd && stringStart > wordStart)
                            {
                                elementNameStart = stringStart;
                                elementNameEnd = stringEnd;
                                elementStart = stringStart;
                            }
                        }
                        else if (text[i] == '{')
                        {
                            level++;
                        }
                        else if (text[i] == '}')
                        {
                            level--;

                            if (level == blockLevel && elementNameStart >= 0 && elementNameStart < elementNameEnd)
                            {
                                int elementEnd = i + 1;
                                yield return new ModElement(this, $"{kind}::{text.Substring(elementNameStart, elementNameEnd - elementNameStart)}", text.Substring(elementStart, elementEnd - elementStart));
                            }
                        }
                        else if (!char.IsWhiteSpace(text[i]))
                        {
                            mode = Mode.Word;
                            wordStart = i;
                        }
                        break;

                    case Mode.Word:
                        if (char.IsWhiteSpace(text[i]))
                        {
                            mode = Mode.Code;
                            wordEnd = i;
                        }
                        break;

                    case Mode.String:
                        if (text[i] == '"' && text[i - 1] != '\\')
                        {
                            mode = Mode.Code;
                            stringEnd = i + 1;
                        }
                        break;

                    case Mode.Comment:
                        if (text[i] == '\n')
                        {
                            mode = Mode.Code;
                        }
                        break;
                }
            }
        }

        private enum Mode
        {
            Code,
            Word,
            String,
            Comment
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TDCG;

namespace TDCG.TSOHair
{
    public class TSOHairPart
    {
        Regex re = null;
        string text_pattern = null;

        public string Name { get; set; }
        public string TextPattern
        {
            get
            {
                return text_pattern;
            }
            set
            {
                text_pattern = value;
                re = new Regex(text_pattern, RegexOptions.IgnoreCase);
            }
        }
        public string SubPath { get; set; }
        public string TexPath { get; set; }

        public Regex GetTextRegex()
        {
            return re;
        }
    }

    public class TSOHairProcessor
    {
        List<TSOHairPart> parts = new List<TSOHairPart>();

        public TSOHairProcessor()
        {
            TSOHairPart part;

            // Housen は Kami より優先する。
            // ex. KamiHousen の場合は Housen として処理
            part = new TSOHairPart();
            part.Name = "Housen";
            part.TextPattern = @"housen|w_faceparts";
            part.SubPath = @"Cgfx_kage\Cgfxkage_{0}";
            part.TexPath = @"image_kage\KIT_KAGE_{0}.bmp";
            parts.Add(part);

            part = new TSOHairPart();
            part.Name = "Ribbon";
            part.TextPattern = @"ribb?on";
            part.SubPath = null;
            part.TexPath = @"image_ribon\KIT_RIBON_{0}.bmp";
            parts.Add(part);

            part = new TSOHairPart();
            part.Name = "Kami";
            part.TextPattern = @"kami";
            part.SubPath = @"Cgfx_kami\Cgfxkami_{0}";
            part.TexPath = @"image_kami\KIT_BASE_{0}.bmp";
            parts.Add(part);
        }

        public static string GetHairKitPath()
        {
            return Path.Combine(Application.StartupPath, @"HAIR_KIT");
        }

        public void Process(TSOFile tso, string col)
        {
            Dictionary<string, TSOTex> texmap = new Dictionary<string, TSOTex>();

            foreach (TSOTex tex in tso.textures)
            {
                texmap[tex.Name] = tex;
            }

            foreach (TSOSubScript sub in tso.sub_scripts)
            {
                Console.WriteLine("  sub name {0} file {1}", sub.Name, sub.FileName);
                TSOHairPart detected_part = null;

                Shader shader = sub.shader;
                string color_tex_name = shader.ColorTexName;
                string shade_tex_name = shader.ShadeTexName;

                foreach (TSOHairPart part in parts)
                {
                    if (part.GetTextRegex().IsMatch(sub.Name))
                    {
                        detected_part = part;
                        break;
                    }
                }

                if (detected_part == null)
                {
                    TSOTex colorTex;
                    if (texmap.TryGetValue(color_tex_name, out colorTex))
                    {
                        string file = colorTex.FileName.Trim('"');

                        foreach (TSOHairPart part in parts)
                        {
                            if (part.GetTextRegex().IsMatch(file))
                            {
                                detected_part = part;
                                break;
                            }
                        }
                    }
                }

                Console.WriteLine("    : type {0}", (detected_part != null) ? detected_part.Name : "Unknown");

                if (detected_part != null)
                {
                    if (detected_part.SubPath != null)
                    {
                        sub.Load(Path.Combine(GetHairKitPath(), string.Format(detected_part.SubPath, col)));
                    }

                    if (detected_part.TexPath != null)
                    {
                        TSOTex colorTex;
                        if (texmap.TryGetValue(color_tex_name, out colorTex))
                        {
                            colorTex.Load(Path.Combine(GetHairKitPath(), string.Format(detected_part.TexPath, col)));
                        }
                    }
                }

                sub.GenerateShader();
                Shader new_shader = sub.shader;
                new_shader.ColorTexName = color_tex_name;
                new_shader.ShadeTexName = shade_tex_name;
                sub.SaveShader();

                Console.WriteLine("    shader color tex name {0}", color_tex_name);
                Console.WriteLine("    shader shade tex name {0}", shade_tex_name);
            }

            foreach (TSOTex tex in tso.textures)
            {
                Console.WriteLine("  tex name {0} file {1}", tex.Name, tex.FileName);
            }
        }
    }
}

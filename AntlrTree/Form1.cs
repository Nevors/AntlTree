using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AntlrGrammars.Css;
using AntlrGrammars.Js;
using AntlrGrammars.Html;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using ImageMagick;
using System.IO;

namespace AntlrTree {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
            this.Shown += Form1_Shown;
        }

        private void Form1_Shown(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            DialogResult result;
            do {
                result = ofd.ShowDialog();
            } while (result != DialogResult.OK || ofd.FileName == null);
            string fileName = ofd.FileName;

            FileInfo snakewareLogo = new FileInfo(fileName);

            MessageBox.Show("Bytes before: " + snakewareLogo.Length);

            ImageOptimizer optimizer = new ImageOptimizer();
            optimizer.LosslessCompress(snakewareLogo);

            snakewareLogo.Refresh();
            MessageBox.Show("Bytes after:  " + snakewareLogo.Length);

            //Compressor
            return;
            AntlrFileStream inputStream = new AntlrFileStream(fileName);

            IParseTree tree = null;
            IVocabulary names = null;
            string typeS = fileName.Substring(fileName.LastIndexOf('.') + 1);
            switch (typeS.ToUpper()) {
                case "CSS":
                    tree = new CssParser(new CommonTokenStream(new CssLexer(inputStream))).stylesheet();
                    names = CssParser.DefaultVocabulary;
                    break;
                case "HTML":
                    tree = new HtmlParser(new CommonTokenStream(new HtmlLexer(inputStream))).htmlDocument();
                    names = HtmlParser.DefaultVocabulary;
                    break;
                case "JS":
                    tree = new JsParser(new CommonTokenStream(new JsLexer(inputStream))).program();
                    names = JsParser.DefaultVocabulary;
                    break;
            }
            var node = treeView.Nodes.Add(ToText(tree, names));
            RecursFn(tree, node, (i) => ToText(i, names));
            /*var d = new CommonTokenStream(new HtmlLexer(inputStream));
            names = HtmlParser.DefaultVocabulary;
            d.Fill();
            foreach(var item in d.GetTokens()) {
                treeView.Nodes.Add(names.GetDisplayName(item.Type) + " --- " + item.ToString());
            }*/

        }

        private void RecursFn(IParseTree tree, TreeNode node ,Func<IParseTree,string> toText) {
            for (int i = 0; i < tree.ChildCount; i++) {
                var child = tree.GetChild(i);
                TreeNode newNode = node.Nodes.Add(toText(child));
                RecursFn(tree.GetChild(i), newNode, toText);
            }
        }

        private string ToText(IParseTree tree,IVocabulary names) {
            if (tree is TerminalNodeImpl token) {
                return String.Concat("\"",token.GetText(),"\" - ", names.GetDisplayName(token.Symbol.Type));
            }
            return tree.GetType().Name.ToString();
        }
    }
}

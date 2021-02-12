using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using MeetingClientWPF.GUI.Controllers.Exceptions;
using MeetingClientWPF.GUI.WinForm;
using Microsoft.VisualBasic.CompilerServices;

namespace MeetingClientWPF.GUI.Controllers
{
    public static class ContextController
    {
        private static bool _lazyAllocation = true; //Alloc when first use
        private static bool _lowMemory = false; //delete if not use

        private static bool _isInit = false;

        private static sbyte _actual = 0, min = -1, max = 1;
        
        private static WindowsFormsHost host = null;
        private static Grid _context = null;
        private static IWPFWindow _controlWindow = null;

        //Layers < 0 - connect
        //Layers = 0 - main
        //Layers > 0 - create
        private static Dictionary<sbyte, Form> _forms;

        private static void _setUpForm(Form form)
        {
            form.TopLevel = false;
            form.TopMost = true;
            form.Dock = DockStyle.Fill;
            form.FormBorderStyle = FormBorderStyle.None;
            form.AutoScaleMode = AutoScaleMode.None;
        }

        private static void _isInitialized()
        {
            if (!_isInit)
            {
                throw new IncompleteInitialization();
            }
        }

        public static void Init(in Grid contextGrid, in IWPFWindow controlWindow, in bool optLazyAlloc = true, in bool optLowMemory = false)
        {
            if (contextGrid == null) throw new ArgumentNullException(nameof(contextGrid) + " = null");
            if (controlWindow == null) throw new ArgumentNullException(nameof(controlWindow) + " = null");

            _lazyAllocation = optLazyAlloc;
            _lowMemory = optLowMemory;
            _context = contextGrid;
            _controlWindow = controlWindow;
            if(!optLazyAlloc) //ak nie je lazy -> vytvor vsetky
            {
                host = new WindowsFormsHost();
                _forms = new Dictionary<sbyte, Form>();
                _forms.Add(0, new FormMain());
            }
            _isInit = true;
        }

        private static void _firstUse()
        {
            host ??= new WindowsFormsHost();
            _forms = new Dictionary<sbyte, Form>();
        }

        private static void _show(Form form)
        {
            _setUpForm(form);
            host.Child = form;
        }

        private static void _showMain() //INDEX:0
        {
            _forms[0] ??= new FormMain();
            _show(_forms[0]);
        }

        private static void _showConnect1() //INDEX:-1
        {
            _forms[-1] ??= new FormConnect_1();
            _show(_forms[-1]);
        }

        private static void _clear()
        {
            if (!_lowMemory) return;

            foreach (var pair in _forms.Where(pair => pair.Key != _actual).Where(pair => pair.Value != null))
            {
                _forms[pair.Key] = null;
            }
        }

        public static void Show(sbyte form)
        {
            _isInitialized();
            _firstUse();
            if (!_forms.ContainsKey(form) && form >= min && form <= max) _forms.Add(form, null);

            _context.Children.Clear();

            switch (form)
            {
                case 0:
                    _showMain();
                    break;
                case -1:
                    _showConnect1();
                    break;
            }

            _context.Children.Add(host);
            _clear();
        }

        //false = negative, true = positive
        public static void Go(bool direction)
        {
            _actual += direction ? 1 : -1;
            if (_actual > max || _actual < min)
            {
                _actual -= _actual > max ? 1 : 0;
                _actual += _actual < min ? 1 : 0;
                throw new NoNextForm();
            }
        
            Show(_actual);
        }

        public static void Back()
        {
            _isInitialized();
            _firstUse();

            _actual -= _actual > 0 ? 1 : 0;
            _actual += _actual < 0 ? 1 : 0;

            Show(_actual);
        }

        public static void BackToStart()
        {
            _isInitialized();
            _firstUse();

            _actual = 0;
            Show(0);
        }

        public static void ExitWinForm(bool type)
        {
            _clear();
            _controlWindow.EndWinForm(type);
        }

        public static void ShowPage(Page page)
        {
            if (page == null) throw new ArgumentNullException(nameof(page) + " = null");
            _isInitialized();
            _firstUse();

            _controlWindow.ShowPage(page);
        }
    }
}

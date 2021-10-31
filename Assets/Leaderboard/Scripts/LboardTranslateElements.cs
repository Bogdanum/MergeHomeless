using UnityEngine;
using UnityEngine.UI;

public class LboardTranslateElements : Singleton<LboardTranslateElements>
{
    [Header("Input Name Window")]
        [SerializeField] private Text inp_window_button_ok;
        [SerializeField] private Text inp_window_error;
        [SerializeField] private Text inp_window_forb;
        [SerializeField] private Text inp_window_header;

    [Header("Leaderboard Access Panel")]
        [SerializeField] private Text lb_access_panel_loadinglb;
        [SerializeField] private Text lb_access_panel_show_around_players;
        [SerializeField] private Text lb_access_panel_header;

    [Header("Leaderboard Access Panel")]
        [SerializeField] private Text lb_panel_loadinglb;
        [SerializeField] private Text lb_panel_show_around_players;
        [SerializeField] private Text lb_panel_show_all_players;
        [SerializeField] private Text lb_panel_header;


    protected override void Awake()
    {
        base.Awake();
        RefreshLanguage();
    }

    public void RefreshLanguage()
    {
        inp_window_button_ok.text                   =    ApplicationLanguage.Text_inp_window_button_ok;               
        inp_window_error.text                       =    ApplicationLanguage.Text_inp_window_error;                   
        inp_window_forb.text                        =    ApplicationLanguage.Text_inp_window_forb;                    
        inp_window_header.text                      =    ApplicationLanguage.Text_inp_window_header;                  
                                                                                                                     
        lb_panel_loadinglb.text                     =    ApplicationLanguage.Text_lb_panel_loadinglb;                 
        lb_panel_show_around_players.text           =    ApplicationLanguage.Text_lb_panel_show_around_players;       
        lb_panel_show_all_players.text              =    ApplicationLanguage.Text_lb_panel_show_all_players;          
        lb_panel_header.text                        =    ApplicationLanguage.Text_lb_panel_header;                    
                                                                                                                     
        lb_access_panel_loadinglb.text              =    ApplicationLanguage.Text_lb_access_panel_loadinglb;          
        lb_access_panel_show_around_players.text    =    ApplicationLanguage.Text_lb_access_panel_show_around_players;
        lb_access_panel_header.text                 =    ApplicationLanguage.Text_lb_access_panel_header;

    }
}

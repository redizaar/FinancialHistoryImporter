   M S B A M L   `   `   `  ����B  >WpfApp1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:WpfApp1WpfApp1  5   	       WpfApp1.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   'localclr-namespace:WpfApp1     5   	     � Title$  Money Manager��5   	   $	��368��6   $	��880��6,    � Icon$ MoneyIcon.png��68   .��5	      � $	��337��6   $	��Top=�6   ���#ECECEA.+�5
   
    � -   5          �Name$ 	MainFrame��5
      $��
150,70,0,0q�6#     � NavigationUIVisibility$ Hidden=�67   
���   �$��0q�6k   ]� 5   
   $��
0,70,730,0q�6   ���#ECECEA.+�5      �� $��	0,0,730,0q�6   ._�5      �� $��	0,0,570,0q�6   .+�5      �� ���#D9853B$	��150��66   $��	-90,0,0,0q�6B   ��5      �� 5       �� 
TargetType" ��6$    �� Triggers 5   "   :� 5   &    :� RoutedEvent$ 
MouseEnter��63   [�5   *   �� 5   .   .��5   2   �� .��5   6   t�  �� TargetProperty$ Margin�6I    t� From$ 	-90,0,0,0X�6l   		 t� To$	 0,0,0,0X�6}   
 g� DecelerationRatio$
 .4Q�6�    g� Duration$ 0:0:0.5D�6�   5   3   5   /   5   '   :� 5   &   $ 
MouseLeave��63   [�5   *   �� 5   .   .��5   2   �� .��5   6   t� $ Margin�6I   $ 0,0,0,0X�6l   $	 	-90,0,0,0X�6{   $
 .9Q�6�   $ 0:0:0.5D�6�   5    3   5!   /   5#   '   5%      .+�5'      ]� -   5         ]�Name$ 
importDock��5'   5   $��0,0,0,0q�6$   .+�5(      �� -   5         ��Name$ ImportButton��5(   >   $��50��6&   $	��150��62   $��Import��6T   ��6e   ��  �� Path$ ImportPushed���5)   "   �� 5*   &   .R�5+   *   �� .+�5,   .   �� $��attachIcon2.png��64   $��30��6M    �� Right$ 15��6Y   
 �� Top$ 8��6k   �� 5-   .    �� Left$ 20��68   $ 13��6I   $��Import��6Y   $��Bookman Old Style)�6g   $��14(�6�   
�������$��0,5,0,0q�6�   5.   +   5/   '   51      52      ]� 53      -   5        $ 	tableDock��53   5   $��0,0,0,0q�6$   .+�54      �� -   5        $ TableButton��54   =   $��50��6%   $	��150��61   $��Database��6R   ��6e   �� $ TablePushed���55   "   �� 56   &   .R�57   *   �� .+�58   .   �� $��tableIcon8.png��64   $��30��6L   $ 15��6X   $ 8��6j   �� 59   .   $ 20��68   $ 13��6I   $��Databse��6Y   $��Bookman Old Style)�6h   $��14(�6�   
�������$��0,5,0,0q�6�   5:   +   5;   '   5=      5>      ]� 5?      -   5        $ stockChartDock��5?   5   $��0,0,0,0q�6$   .+�5@      �� -   5        $ StockChartButton��5@   =   $��50��6%   $	��150��61   $��stockMarketData��6W   ��6q   �� $ PortfolioPushed���5A   "   �� 5B   &   .R�5C   *   �� .+�5D   .   �� $��chartIcon.png��64   $��30��6K   $ 15��6W   $ 8��6i   �� 5E   .   $ 20��68   $ 13��6I   $��	Portfolio��6Y   $��Bookman Old Style)�6j   $��14(�6�   
�������$��0,5,0,0q�6�   5F   +   5G   '   5I      5J      ]� 5K      $��0,0,0,0q�6$   .+�5L      �� -   5        $ 
ExitButton��5L   =   $��50��6%   $	��150��61   $
��Exit��6Q   ��6`   �� $ 
ExitPushed���5M   "   �� 5N   &   .R�5O   *   �� .+�5P   .   �� $��exitIcon5.png��64   $��30��6K   $ 15��6W   $ 8��6i   �� 5Q   .   $ 20��68   $ 13��6I   $
��Exit��6Y   $��Bookman Old Style)�6e   $��14(�6�   
�������$��0,5,0,0q�6�   5R   +   5S   '   5U      5V      5W      5X      5Y      5Z      �� 5[   
   
���   �$��1q�6%   $
��Left=�69   $��70��6T   $	��Top=�6`   $	��150��6x   ���#2c3e50._�5\      �� .+�5]      ]� .+�5^      �� $��	Financial��6   $��15(�60   $��40,3,0,0q�6>   
�������$��Bookman Old Style)�6c   5_      �� 5`      $��History Importer��6   $��15(�63   $��10,0,0,0q�6A   
�������$��Bookman Old Style)�6f   5a      5b      �� 5c   
   -	   5         ��Name$ importMenuTop��5c      
���   �$��0,0,0,1q�6<   $
��Left=�6V   $��70��6q   $��	150,0,0,0q�6}   $	��Top=�6�   $	��639��6�   ���#74AFAD._�5d      �� .+�5e      ]� $��0,0,0,0q�6   .+�5f      �� .+�5g      �� $��attachIcon2.png��6    $��70��69   $��
10,5,635,0q�6D   $��55��6X   �� 5h      $#��Import your Financial History��6    $��80,8,0,0q�6H   $��20(�6Z   $��Arial)�6h   
�������6{   5i      5j      �� 5k      $��	85,30,0,0q�6   .+�5l      �� 
���   �$��13(�6/   $;��5In this Menu you Import your Financial History datas.��6=   6   5m      5n      5o      �� 5p   
   -
   5        $ tableMenuTop��5p      
���   �$��0,0,0,1q�6;   $
��Left=�6U   $��70��6p   $��	150,0,0,0q�6|   $	��Top=�6�   $	��639��6�   ���#74AFAD._�5q      �� .+�5r      ]� $��0,0,0,0q�6   .+�5s      �� .+�5t      �� $��tableIcon8.png��6    $��70��68   $��
10,5,635,0q�6C   $��55��6W   �� 5u      $��Transactions in Database��6    $��80,8,0,0q�6C   $��20(�6U   $��Arial)�6c   
�������6v   5v      5w      �� 5x      $��	85,30,0,0q�6   .+�5y      �� 
���   �$��13(�6/   $J��DIn this Menu you can see your Financial History Table from Database.��6=   6�   5z      5{      5|      �� 5}   
   -   5        $ portfolioMenuTop��5}      
���   �$��0,0,0,1q�6?   $
��Left=�6Y   $��70��6t   $��	150,0,0,0q�6�   $	��Top=�6�   $	��639��6�   ���#74AFAD._�5~      �� .+�5      ]� $��0,0,0,0q�6   .+�5�      �� .+�5�      �� $��chartIcon.png��6    $��70��67   $��
10,5,635,0q�6B   $��55��6V   �� 5�      $��Share Pices��6    $��80,8,0,0q�66   $��20(�6H   $��Arial)�6V   
�������6i   5�      5�      �� 5�      $��	85,30,0,0q�6   .+�5�      �� 
���   �$��13(�6/   $R��LIn this Menu you can see different share prices updated from Google Finance.��6=   6�   5�      5�      5�      �� 5�   
   -   5        $ userNotification��5�      
���   �$��1,0,0,1q�6?   $
��Left=�6Y   $��70��6t   $��	785,0,0,0q�6�   $	��Top=�6�   $��87��6�   ���#74AFAD._�5�      �� $��
-21,0,35,0q�6   .+�5�      �� .+�5�      �� $��alarmIcon3.png��6   $��70��64   $��30,5,0,0q�6?   $��45��6Q   5�      5�      5�      �� 5�   
   -   5         ��Name$ exclamImage��5�      $��exclamIcon.png��6%   $��20��6=   $��20��6J   $��Right=�6U   $��
0,-320,0,0q�6q   �� 5�   
   -   5         ��Name$ currentUserLabel��5�      $
��User��6*   $��800,40,0,267q�69   $
��Bold"�6O    � 5�   
   -   5        $ 
LoginFrame��5�      $
��Left=�6$   $	��337��6?   $	��Top=�6L   $	��880��6d   $ Hidden=�6p   5�      5�      
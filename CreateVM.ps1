#Prepare the VM parameters
 $rgName = "gorilla-apps-qc-resourcegroup"
 $location = "East US"
 $vnet = "GK-VN-01/default"
 $subnet = "/subscriptions/5365a998-80ee-4ac0-b274-377d635e967b/resourceGroups/gorilla-apps-qc-resourcegroup/provider
s/Microsoft.Network/virtualNetworks/GK-VN-01/subnets/default"
 $nicName = "VM02-GK"
 $vmName = "VM01"
 $osDiskName = "VM01-OSDisk"
 $osDiskUri = "https://gkvmsnaphot.blob.core.windows.net/vhds/GKVM20170929142318.vhd"
 $VMSize = "Standard_A1"
 $storageAccountType = "StandardLRS"
 $IPaddress = 10.1.0.5

 #Create the VM resources
 $IPconfig = New-AzureRmNetworkInterfaceIpConfig -Name IPConfig1 -PrivateIpAddressVersion IPv4 -PrivateIpAddress $IPaddress -SubnetId $subnet
 $nic = New-AzureRmNetworkInterface -Name $nicName -ResourceGroupName $rgName -Location $location -IpConfiguration $IPconfig
 $vmConfig = New-AzureRmVMConfig -VMName $vmName -VMSize $VMSize
 $vm = Add-AzureRmVMNetworkInterface -VM $vmConfig -Id $nic.Id

 $osDisk = New-AzureRmDisk -DiskName $osDiskName -Disk (New-AzureRmDiskConfig -AccountType $storageAccountType -Location $location -CreateOption Import -SourceUri $osDiskUri) -ResourceGroup Name $rgName
 $vm = Set-AzureRmVMOSDisk -VM $vm -ManagedDiskId $osDisk.Id -StorageAccountType $storageAccountType -DiskSizeInGB 128 -CreateOption Attach -Windows
 $vm = Set-AzureRmVMBootDiagnostics -VM $vm -disable

 #Create the new VM
 New-AzureRmVM -ResourceGroupName $rgName -Location $location -VM $vm
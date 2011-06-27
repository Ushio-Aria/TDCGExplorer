using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using TDCG;
using TDCGUtils;
using Tso2Pmd;

namespace TDCG.PhysObTemplate
{
    public class Chichi_Hard : IPhysObTemplate
    {
        string name = "�����";   // �g�ɕ\������閼�O
        int group = 1;          // �\�������g�̎�ށi0:�� 1:�� 2:�X�J�[�g 3:���̑��j

        public string Name() { return name; }
        public int Group() { return group; }

        public void Execute(ref T2PPhysObjectList phys_list)
        {
            phys_list.MakeChain("�E���P");
            phys_list.MakeChain("�����P");
        
            SetParameter(phys_list.GetBodyListByName(".��."));
            SetParameterEnd(phys_list.GetBodyListByName(".����"));
            SetParameter(phys_list.GetJointListByName(".��."));
            phys_list.GetBodyByName("�E���P").rigidbody_type = 0; // ���f�[�^�F�^�C�v(0:Bone�Ǐ]�A1:�������Z�A2:�������Z(Bone�ʒu����)) // 00 // Bone�Ǐ]
            phys_list.GetBodyByName("�����P").rigidbody_type = 0; // ���f�[�^�F�^�C�v(0:Bone�Ǐ]�A1:�������Z�A2:�������Z(Bone�ʒu����)) // 00 // Bone�Ǐ]
            phys_list.GetBodyByName("�E���Q").rigidbody_type = 0; // ���f�[�^�F�^�C�v(0:Bone�Ǐ]�A1:�������Z�A2:�������Z(Bone�ʒu����)) // 00 // Bone�Ǐ]
            phys_list.GetBodyByName("�����Q").rigidbody_type = 0; // ���f�[�^�F�^�C�v(0:Bone�Ǐ]�A1:�������Z�A2:�������Z(Bone�ʒu����)) // 00 // Bone�Ǐ]
        }

        private void SetParameter(List<PMD_RigidBody> body_list)
        {
            foreach (PMD_RigidBody body in body_list)
            {
                body.rigidbody_group_index = 3; // ���f�[�^�F�O���[�v // 00
                body.rigidbody_group_target = 1; // ���f�[�^�F�O���[�v�F�Ώ� // 0xFFFF�Ƃ̍� // 38 FE
                body.shape_type = 2; // �`��F�^�C�v(0:���A1:���A2:�J�v�Z��) // 00 // ��
                body.shape_w = 0.2f; // �`��F���a(��) // CD CC CC 3F // 1.6

                body.rigidbody_weight = 0.1f; // ���f�[�^�F���� // 00 00 80 3F // 1.0
                body.rigidbody_pos_dim = 0.5f; // ���f�[�^�F�ړ��� // 00 00 00 00
                body.rigidbody_rot_dim = 0.5f; // ���f�[�^�F��]�� // 00 00 00 00
                body.rigidbody_recoil = 0.0f; // ���f�[�^�F������ // 00 00 00 00
                body.rigidbody_friction = 0.0f; // ���f�[�^�F���C�� // 00 00 00 00
                body.rigidbody_type = 1; // ���f�[�^�F�^�C�v(0:Bone�Ǐ]�A1:�������Z�A2:�������Z(Bone�ʒu����)) // 00 // Bone�Ǐ]
            }
        }

        private void SetParameterEnd(List<PMD_RigidBody> body_list)
        {
            foreach (PMD_RigidBody body in body_list)
            {
                body.rigidbody_group_index = 3; // ���f�[�^�F�O���[�v // 00
                body.rigidbody_group_target = 1; // ���f�[�^�F�O���[�v�F�Ώ� // 0xFFFF�Ƃ̍� // 38 FE
                body.shape_type = 0; // �`��F�^�C�v(0:���A1:���A2:�J�v�Z��) // 00 // ��
                body.shape_w = 0.2f; // �`��F���a(��) // CD CC CC 3F // 1.6

                body.rigidbody_weight = 0.01f; // ���f�[�^�F���� // 00 00 80 3F // 1.0
                body.rigidbody_pos_dim = 0.5f; // ���f�[�^�F�ړ��� // 00 00 00 00
                body.rigidbody_rot_dim = 0.5f; // ���f�[�^�F��]�� // 00 00 00 00
                body.rigidbody_recoil = 0.0f; // ���f�[�^�F������ // 00 00 00 00
                body.rigidbody_friction = 0.0f; // ���f�[�^�F���C�� // 00 00 00 00
                body.rigidbody_type = 1; // ���f�[�^�F�^�C�v(0:Bone�Ǐ]�A1:�������Z�A2:�������Z(Bone�ʒu����)) // 00 // Bone�Ǐ]
            }
        }

        private void SetParameter(List<PMD_Joint> joint_list)
        {
            foreach (PMD_Joint joint in joint_list)
            {
                joint.spring_rot.X = 200.0f; // �΂ˁF��](rad(x), rad(y), rad(z))
                joint.spring_rot.Y = 200.0f; // �΂ˁF��](rad(x), rad(y), rad(z))
                joint.spring_rot.Z = 200.0f; // �΂ˁF��](rad(x), rad(y), rad(z))
            }
        }
    }
}